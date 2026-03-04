module internal Shopping.BlobStorage.Repository.Common.Utils

open System
open System.IO
open System.Collections.Generic
open Azure
open Azure.Storage.Blobs
open Azure.Storage.Blobs.Models
open Shopping.Common.Types
open Shopping.Models.Dto.FileDto

let private mapFileStorageException (ex: RequestFailedException) =
    match ex.Status with
    | 404 -> DataNotFound "Requested file resource was not found."
    | 409 -> Conflict "A conflicting file resource already exists."
    | 400 -> BadRequest "The file request is invalid."
    | 401 -> Unauthorized "Access to file resources is unauthorized."
    | 403 -> Forbidden "Access to file resources is forbidden."
    | 412 -> PreconditionFailed "File operation precondition failed."
    | 429 -> TooManyRequests "Too many file requests. Please retry later."
    | 503 -> ServiceUnavailable "File service is temporarily unavailable."
    | _ -> ExceptionWithStatusCode (ex.Status, "File storage operation failed.")

let getBlobPath file =
    if String.IsNullOrEmpty(file.Folder) then
        file.FileName
    else
        file.Folder + "/" + file.FileName

let getBlobMetadata (file: FileDto) =
    match file.Metadata with
    | None -> null
    | Some metadata ->
        let dictionary = Dictionary<string, string>()
        match metadata.ProductId with
        | Some productId -> dictionary["productId"] <- productId
        | None -> ()
        match metadata.ProductCategory with
        | Some productCategory -> dictionary["productCategory"] <- productCategory
        | None -> ()
        if dictionary.Count = 0 then null else dictionary

let uploadFileAsync (containerClient: BlobContainerClient) (file: FileDto) (content: Stream) =
    task {
        try
            let blobPath = getBlobPath file
            let blobClient = containerClient.GetBlobClient(blobPath)
            let options =
                BlobUploadOptions(
                    HttpHeaders = BlobHttpHeaders(ContentType = file.ContentType),
                    Metadata = getBlobMetadata file
                )
            let! _ = blobClient.UploadAsync(content, options)
            return Success ()
        with
        | :? RequestFailedException as ex -> return Failure (mapFileStorageException ex)
        | _ -> return Failure (Exception "Unexpected file operation error.")
    }

let downloadFileAsync (containerClient: BlobContainerClient) (file: FileDto) =
    task {
        try
            let blobPath = getBlobPath file
            let blobClient = containerClient.GetBlobClient(blobPath)
            let! response = blobClient.DownloadAsync()
            return Success response.Value.Content
        with
        | :? RequestFailedException as ex -> return Failure (mapFileStorageException ex)
        | _ -> return Failure (Exception "Unexpected file operation error.")
    }

let updateFileAsync (containerClient: BlobContainerClient) (file: FileDto) (content: Stream) =
    task {
        try
            let blobPath = getBlobPath file
            let blobClient = containerClient.GetBlobClient(blobPath)
            let! _ = blobClient.UploadAsync(content, overwrite = true)
            let headers = BlobHttpHeaders(ContentType = file.ContentType)
            let! _ = blobClient.SetHttpHeadersAsync(headers)
            match getBlobMetadata file with
            | null -> ()
            | metadata ->
                let! _ = blobClient.SetMetadataAsync(metadata)
                ()
            return Success ()
        with
        | :? RequestFailedException as ex -> return Failure (mapFileStorageException ex)
        | _ -> return Failure (Exception "Unexpected file operation error.")
    }

let deleteFileAsync (containerClient: BlobContainerClient) (file: FileDto) =
    task {
        try
            let blobPath = getBlobPath file
            let blobClient = containerClient.GetBlobClient(blobPath)
            let! response = blobClient.DeleteIfExistsAsync()
            if response.Value then
                return Success ()
            else
                return Failure (DataNotFound "Requested file resource was not found.")
        with
        | :? RequestFailedException as ex -> return Failure (mapFileStorageException ex)
        | _ -> return Failure (Exception "Unexpected file operation error.")
    }

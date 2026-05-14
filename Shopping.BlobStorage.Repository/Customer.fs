module Shopping.Files.Repository.Customer

open System.IO
open System.Threading.Tasks
open Azure.Storage.Blobs
open Shopping.BlobStorage.Common.ServiceProviderFactory
open Shopping.BlobStorage.Repository.Common.Utils
open Shopping.Common.Types
open Shopping.Models.Domain.File

let private getContainer () =
    getService<BlobServiceClient>().GetBlobContainerClient("customers-files")

let uploadFile : File -> Stream -> _ =
    uploadFileAsync (getContainer ())

let downloadFile =
    downloadFileAsync (getContainer ())

let updateFile : File -> Stream -> _ =
    updateFileAsync (getContainer ())

let deleteFile =
    deleteFileAsync (getContainer ())

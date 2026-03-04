module Shopping.Files.Repository.Product

open System.IO
open System.Threading.Tasks
open Azure.Storage.Blobs
open Shopping.BlobStorage.Common.ServiceProviderFactory
open Shopping.BlobStorage.Repository.Common.Utils
open Shopping.Common.Types
open Shopping.Models.Dto.FileDto

let private getContainer () =
    getService<BlobServiceClient>().GetBlobContainerClient("products-files")

let uploadFile: FileDto -> Stream -> Task<Result<unit, ReadItemFailureReason>> =
    uploadFileAsync (getContainer ())

let downloadFile =
    downloadFileAsync (getContainer ())
    
let updateFile: FileDto -> Stream -> Task<Result<unit, ReadItemFailureReason>>  =
    updateFileAsync (getContainer ())
    
let deleteFile =
    deleteFileAsync (getContainer ())
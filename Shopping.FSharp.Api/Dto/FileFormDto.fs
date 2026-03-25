namespace Shopping.FSharp.Api.Dto

open Microsoft.AspNetCore.Http
[<CLIMutable>]
type FileMetadataFormDto = {
    ProductId: string option
    ProductCategory: string option
}

[<CLIMutable>]
type FileFormDto = {
    FileName: string
    ContentType: string
    Folder: string
    Metadata: FileMetadataFormDto
    File: IFormFile
}

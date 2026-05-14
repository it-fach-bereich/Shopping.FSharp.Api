module Shopping.FSharp.Api.Dto.File

type FileMetadataDto = {
    ProductId: string option
    ProductCategory: string option
}

type FileDto = {
    FileName: string
    Folder: string
    Size: int64
    ContentType: string
    Metadata: FileMetadataDto option
}

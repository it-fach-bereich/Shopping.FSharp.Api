module Shopping.Models.Dto.FileDto

type FileMetadata = {
    ProductId: string option
    ProductCategory: string option
}

type FileDto = {
    FileName: string
    Folder: string
    Size: int64
    ContentType: string
    Metadata: FileMetadata option
}

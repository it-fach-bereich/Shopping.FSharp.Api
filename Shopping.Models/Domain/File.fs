module Shopping.Models.Domain.File

type FileMetadata = {
    ProductId: string option
    ProductCategory: string option
}

type File = {
    FileName: string
    Folder: string
    Size: int64
    ContentType: string
    Metadata: FileMetadata option
}

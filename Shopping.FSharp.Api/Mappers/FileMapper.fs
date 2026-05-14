module Shopping.FSharp.Api.Mappers.FileMapper

open Shopping.Models.Domain.File
open Shopping.FSharp.Api.Dto.File

let toEntity (dto: FileDto) : File =
    {
        FileName = dto.FileName
        Folder = dto.Folder
        Size = dto.Size
        ContentType = dto.ContentType
        Metadata =
            dto.Metadata
            |> Option.map (fun m -> {
                ProductId = m.ProductId
                ProductCategory = m.ProductCategory
            })
    }

let toDto (file: File) : FileDto =
    {
        FileName = file.FileName
        Folder = file.Folder
        Size = file.Size
        ContentType = file.ContentType
        Metadata =
            file.Metadata
            |> Option.map (fun m -> {
                ProductId = m.ProductId
                ProductCategory = m.ProductCategory
            })
    }

module Shopping.FSharp.Api.Mappers.ProductMapper

open Shopping.Models.Domain.Product
open Shopping.FSharp.Api.Dto.Product
open Shopping.FSharp.Api.Dto.ProductSearch

let toDto (product: Product) : ProductDto =
    {
        Id = product.Id
        Category = product.Category
        Name = product.Name
        Quantity = product.Quantity
        Price = product.Price
        Clearance = product.Clearance
    }

let toEntity (dto: ProductDto) : Product =
    {
        Id = dto.Id
        Category = dto.Category
        Name = dto.Name
        Quantity = dto.Quantity
        Price = dto.Price
        Clearance = dto.Clearance
    }

let toSearch (dto: ProductSearchDto) : ProductSearch =
    {
        Category  = Option.ofObj dto.Category
        Name      = Option.ofObj dto.Name
        MinPrice  = Option.ofNullable dto.MinPrice
        MaxPrice  = Option.ofNullable dto.MaxPrice
        Clearance = Option.ofNullable dto.Clearance
    }

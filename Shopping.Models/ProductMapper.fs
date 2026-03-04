module Shopping.Models.ProductMapper

open Shopping.Models.Domain.Product
open Shopping.Models.Dto.Product

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

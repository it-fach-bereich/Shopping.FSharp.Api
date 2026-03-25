module Shopping.Models.Dto.Product

type ProductDto = {
    Id: string
    Category: string
    Name: string
    Quantity: int
    Price: decimal
    Clearance: bool
}


module Shopping.Models.Domain.Product

open Newtonsoft.Json

type Product = {
    [<JsonProperty("id")>]
    Id: string
    [<JsonProperty("category")>]
    Category: string
    [<JsonProperty("name")>]
    Name: string
    [<JsonProperty("quantity")>]
    Quantity: int
    [<JsonProperty("price")>]
    Price: decimal
    [<JsonProperty("clearance")>]
    Clearance: bool
}

type ProductSearch = {
    Category: string option
    Name: string option
    MinPrice: decimal option
    MaxPrice: decimal option
    Clearance: bool option
}


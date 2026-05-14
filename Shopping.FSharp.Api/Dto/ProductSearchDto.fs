module Shopping.FSharp.Api.Dto.ProductSearch

open System

[<CLIMutable>]
type ProductSearchDto = {
    Category: string
    Name: string
    MinPrice: Nullable<decimal>
    MaxPrice: Nullable<decimal>
    Clearance: Nullable<bool>
}

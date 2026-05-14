module Shopping.FSharp.Api.Dto.OrderSearch

open System

[<CLIMutable>]
type OrderSearchDto = {
    CustomerId: string
    ProductId: string
    OrderedFrom: Nullable<DateTimeOffset>
    OrderedTo: Nullable<DateTimeOffset>
}

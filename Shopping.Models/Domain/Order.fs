module Shopping.Models.Domain.Order

open System
open Newtonsoft.Json

type OrderItem = {
    [<JsonProperty("productId")>]
    ProductId: string
    [<JsonProperty("quantity")>]
    Quantity: int
    [<JsonProperty("unitPrice")>]
    UnitPrice: decimal
}

type Order = {
    [<JsonProperty("id")>]
    Id: string
    [<JsonProperty("customerId", Required = Required.Always)>]
    CustomerId: string
    [<JsonProperty("items")>]
    Items: OrderItem list
    [<JsonProperty("orderedAt")>]
    OrderedAt: DateTimeOffset
    [<JsonProperty("totalAmount")>]
    TotalAmount: decimal
}

type OrderSearch = {
    CustomerId: string option
    ProductId: string option
    OrderedFrom: DateTimeOffset option
    OrderedTo: DateTimeOffset option
}

type PlaceOrderItemRequest = {
    ProductId: string
    Quantity: int
}

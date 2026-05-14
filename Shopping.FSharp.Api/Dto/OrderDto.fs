module Shopping.FSharp.Api.Dto.Order

open System

type OrderItemDto = {
    ProductId: string
    Quantity: int
    UnitPrice: decimal
}

type OrderDto = {
    Id: string
    CustomerId: string
    Items: OrderItemDto list
    OrderedAt: DateTimeOffset
    TotalAmount: decimal
}

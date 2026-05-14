module Shopping.FSharp.Api.Mappers.OrderMapper

open Shopping.Models.Domain.Order
open Shopping.FSharp.Api.Dto.Order
open Shopping.FSharp.Api.Dto.OrderSearch
open Shopping.FSharp.Api.Dto.PlaceOrderItemRequest
open Shopping.FSharp.Api.Dto.PlaceOrderRequest

let toItemDto (item: OrderItem) : OrderItemDto =
    {
        ProductId = item.ProductId
        Quantity  = item.Quantity
        UnitPrice = item.UnitPrice
    }

let toDto (order: Order) : OrderDto =
    {
        Id          = order.Id
        CustomerId  = order.CustomerId
        Items       = order.Items |> List.map toItemDto
        OrderedAt   = order.OrderedAt
        TotalAmount = order.TotalAmount
    }

let toItemEntity (dto: OrderItemDto) : OrderItem =
    {
        ProductId = dto.ProductId
        Quantity  = dto.Quantity
        UnitPrice = dto.UnitPrice
    }

let toEntity (dto: OrderDto) : Order =
    {
        Id          = dto.Id
        CustomerId  = dto.CustomerId
        Items       = dto.Items |> List.map toItemEntity
        OrderedAt   = dto.OrderedAt
        TotalAmount = dto.TotalAmount
    }

let toSearch (dto: OrderSearchDto) : OrderSearch =
    {
        CustomerId  = Option.ofObj dto.CustomerId
        ProductId   = Option.ofObj dto.ProductId
        OrderedFrom = Option.ofNullable dto.OrderedFrom
        OrderedTo   = Option.ofNullable dto.OrderedTo
    }

let toPlaceOrderItemRequest (dto: PlaceOrderItemRequestDto) : PlaceOrderItemRequest =
    {
        ProductId = dto.ProductId
        Quantity  = dto.Quantity
    }

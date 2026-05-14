module Shopping.FSharp.Api.Dto.PlaceOrderRequest

open Shopping.FSharp.Api.Dto.PlaceOrderItemRequest

type PlaceOrderRequestDto = {
    CustomerId: string
    Items: PlaceOrderItemRequestDto list
}

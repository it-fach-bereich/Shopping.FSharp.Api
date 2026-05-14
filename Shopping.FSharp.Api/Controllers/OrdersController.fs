namespace Shopping.FSharp.Api.Controllers

open Microsoft.AspNetCore.Mvc
open Shopping.Common.Types
open Shopping.FSharp.Api.Dto.Order
open Shopping.FSharp.Api.Dto.OrderSearch
open Shopping.FSharp.Api.Dto.PlaceOrderRequest
open Shopping.FSharp.Api.Mappers
open Shopping.Order.Domain

[<ApiController>]
[<Route("api/[controller]")>]
type OrdersController() =
    inherit ShoppingControllerBase()

    [<HttpGet>]
    [<Route("")>]
    member this.GetAll() =
        task {
            let! result = getAllOrders
            return
                match result with
                | Success x ->
                    let orders =
                        x
                        |> Seq.collect (fun feed -> feed :> seq<_>)
                        |> Seq.map OrderMapper.toDto
                    this.Ok(orders) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpGet>]
    [<Route("{id}/{customerId}")>]
    member this.GetById(id: string, customerId: string) =
        task {
            let! result = getOrderById id (StringKey customerId)
            return
                match result with
                | Success x -> this.Ok(OrderMapper.toDto x) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpGet>]
    [<Route("customer/{customerId}")>]
    member this.GetByCustomerId(customerId: string) =
        task {
            let! result = getOrdersByCustomerId customerId
            return
                match result with
                | Success x -> this.Ok(x |> List.map OrderMapper.toDto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("search")>]
    member this.Search([<FromBody>] search: OrderSearchDto) =
        task {
            let! result = searchOrders (OrderMapper.toSearch search)
            return
                match result with
                | Success x -> this.Ok(x |> List.map OrderMapper.toDto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("place")>]
    member this.PlaceOrder([<FromBody>] request: PlaceOrderRequestDto) =
        task {
            let items = request.Items |> List.map OrderMapper.toPlaceOrderItemRequest
            let! result = placeOrder request.CustomerId items
            return
                match result with
                | Success x ->
                    let dto = OrderMapper.toDto x
                    this.CreatedAtAction("GetById", {| id = dto.Id; customerId = dto.CustomerId |}, dto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpDelete>]
    [<Route("{id}/{customerId}")>]
    member this.Delete(id: string, customerId: string) =
        task {
            let! result = deleteOrder id (StringKey customerId)
            return
                match result with
                | Success _ -> this.NoContent() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

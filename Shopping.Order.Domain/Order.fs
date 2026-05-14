module Shopping.Order.Domain

open System
open System.Linq
open System.Linq.Expressions
open Shopping.Common.Types
open Shopping.Models.Domain.Customer
open Shopping.Models.Domain.Product
open Shopping.Models.Domain.Order
open Shopping.Data.Repository.Order

let private customerGetById = Shopping.Data.Repository.Customer.getById
let private productFind     = Shopping.Data.Repository.Product.find
let private productUpdate   = Shopping.Data.Repository.Product.update

let getOrderById = getById

let getAllOrders = getAll

let searchOrders (search: OrderSearch) =
    let param = Expression.Parameter(typeof<Order>, "o")
    let conditions =
        [
            search.CustomerId  |> Option.map (fun v ->
                Expression.Equal(
                    Expression.Property(param, "CustomerId"),
                    Expression.Constant(v)) :> Expression)

            search.OrderedFrom |> Option.map (fun v ->
                Expression.GreaterThanOrEqual(
                    Expression.Property(param, "OrderedAt"),
                    Expression.Constant(v)) :> Expression)

            search.OrderedTo   |> Option.map (fun v ->
                Expression.LessThanOrEqual(
                    Expression.Property(param, "OrderedAt"),
                    Expression.Constant(v)) :> Expression)

            search.ProductId   |> Option.map (fun v ->
                let itemParam = Expression.Parameter(typeof<OrderItem>, "i")
                let itemPredicate =
                    Expression.Lambda<Func<OrderItem, bool>>(
                        Expression.Equal(
                            Expression.Property(itemParam, "ProductId"),
                            Expression.Constant(v)),
                        itemParam)
                Expression.Call(
                    typeof<Enumerable>, "Any", [| typeof<OrderItem> |],
                    Expression.Property(param, "Items"),
                    itemPredicate) :> Expression)
        ]
        |> List.choose id
    let body =
        match conditions with
        | [] -> Expression.Constant(true) :> Expression
        | _  -> conditions |> List.reduce (fun acc expr -> Expression.AndAlso(acc, expr) :> Expression)
    let predicate = Expression.Lambda<Func<Order, bool>>(body, param)
    find predicate

let private findProductById (productId: string) =
    let param = Expression.Parameter(typeof<Product>, "p")
    let predicate =
        Expression.Lambda<Func<Product, bool>>(
            Expression.Equal(
                Expression.Property(param, "Id"),
                Expression.Constant(productId)),
            param)
    productFind predicate

let rec private validateProducts (items: (string * int) list) (acc: Map<string, Product>) =
    task {
        match items with
        | [] -> return Success acc
        | (productId, qty) :: rest ->
            if qty <= 0 then
                return Failure (BadRequest $"Quantity for product '{productId}' must be greater than zero.")
            else
                let! result = findProductById productId
                match result with
                | Failure reason -> return Failure reason
                | Success products ->
                    match products with
                    | [] -> return Failure (DataNotFound $"Product '{productId}' was not found.")
                    | product :: _ ->
                        if product.Quantity < qty then
                            return Failure (BadRequest $"Insufficient stock for product '{productId}'. Requested: {qty}, available: {product.Quantity}.")
                        else
                            return! validateProducts rest (acc |> Map.add productId product)
    }

let rec private updateProductQuantities (quantities: (string * int) list) (productsById: Map<string, Product>) =
    task {
        match quantities with
        | [] -> return Success ()
        | (productId, qty) :: rest ->
            let product = productsById.[productId]
            let updated = { product with Quantity = product.Quantity - qty }
            let! result = productUpdate updated
            match result with
            | Failure reason -> return Failure reason
            | Success _ -> return! updateProductQuantities rest productsById
    }

let getOrdersByCustomerId (customerId: string) =
    let param = Expression.Parameter(typeof<Order>, "o")
    let predicate =
        Expression.Lambda<Func<Order, bool>>(
            Expression.Equal(
                Expression.Property(param, "CustomerId"),
                Expression.Constant(customerId)),
            param)
    find predicate

let placeOrder (customerId: string) (items: PlaceOrderItemRequest list) =
    task {
        match items with
        | [] -> return Failure (BadRequest "Order must contain at least one item.")
        | _ ->

        let! customerResult = customerGetById customerId (StringKey customerId)
        match customerResult with
        | Failure reason -> return Failure reason
        | Success _ ->

        let requestedQuantities =
            items
            |> List.groupBy (fun i -> i.ProductId)
            |> List.map (fun (id, grp) -> id, grp |> List.sumBy (fun i -> i.Quantity))

        let! productsResult = validateProducts requestedQuantities Map.empty
        match productsResult with
        | Failure reason -> return Failure reason
        | Success productsById ->

        let! updateResult = updateProductQuantities requestedQuantities productsById
        match updateResult with
        | Failure reason -> return Failure reason
        | Success _ ->

        let pricedItems =
            items |> List.map (fun item ->
                let product = productsById.[item.ProductId]
                { ProductId = item.ProductId
                  Quantity  = item.Quantity
                  UnitPrice = product.Price })

        let totalAmount = pricedItems |> List.sumBy (fun i -> i.UnitPrice * decimal i.Quantity)

        let order = {
            Id          = Guid.NewGuid().ToString("N")
            CustomerId  = customerId
            Items       = pricedItems
            OrderedAt   = DateTimeOffset.UtcNow
            TotalAmount = totalAmount
        }

        return! add order
    }

let deleteOrder = delete

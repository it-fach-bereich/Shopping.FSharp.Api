module Shopping.Data.Repository.Order

open System
open System.Linq.Expressions
open Shopping.Data.Repository.Common.ServiceProviderFactory
open Microsoft.Azure.Cosmos
open Shopping.Data.Repository.Common.Utils
open Shopping.Models.Domain.Order

let private shoppingDb = "ShoppingDb"
let private ordersContainer = "Orders"
let private getContainer () =
    getService<CosmosClient>().GetContainer(shoppingDb, ordersContainer)

let getById =
    getByIdAsync<Order> (getContainer ())

let getAll =
    getAllAsync<Order> (getContainer ())

let find (predicate: Expression<Func<Order, bool>>) =
    findAsync<Order> (getContainer ()) predicate

let add =
    addAsync<Order> (getContainer ())

let update =
    updateAsync<Order> (getContainer ())

let delete =
    deleteAsync<Order> (getContainer ())

module Shopping.Data.Repository.Customer

open System.Linq.Expressions
open System
open Shopping.Data.Repository.Common.ServiceProviderFactory
open Microsoft.Azure.Cosmos
open Shopping.Data.Repository.Common.Utils
open Shopping.Models.Domain.Customer

let private shoppingDb = "ShoppingDb"
let private customersContainer = "Customers"
let private getContainer () =
    getService<CosmosClient>().GetContainer(shoppingDb, customersContainer)

let getById =
    getByIdAsync<Customer> (getContainer ())

let getAll =
    getAllAsync<Customer> (getContainer ())

let find (predicate: Expression<Func<Customer, bool>>) =
    findAsync<Customer> (getContainer ()) predicate

let add =
    addAsync<Customer> (getContainer ())

let update =
    updateAsync<Customer> (getContainer ())

let delete =
    deleteAsync<Customer> (getContainer ())

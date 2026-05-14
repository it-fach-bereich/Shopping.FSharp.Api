module Shopping.Data.Repository.Product

open System
open System.Linq.Expressions
open Shopping.Data.Repository.Common.ServiceProviderFactory
open Microsoft.Azure.Cosmos
open Shopping.Data.Repository.Common.Utils
open Shopping.Models.Domain.Product

let private shoppingDb = "ShoppingDb"
let private productsContainer = "Products"
let private getContainer () =
    getService<CosmosClient>().GetContainer(shoppingDb, productsContainer)

let getById =
    getByIdAsync<Product> (getContainer ())

let getAll =
    getAllAsync<Product> (getContainer ())

let find (predicate: Expression<Func<Product, bool>>) =
    findAsync<Product> (getContainer ()) predicate

let add =
    addAsync<Product> (getContainer ())

let update =
    updateAsync<Product> (getContainer ())

let delete =
    deleteAsync<Product> (getContainer ())
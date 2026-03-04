module Shopping.Data.Repository.Product

open Shopping.Data.Repository.Common.ServiceProviderFactory
open Microsoft.Azure.Cosmos
open Shopping.Data.Repository.Common.Utils

let private shoppingDb = "ShoppingDb"
let private productsContainer = "Products"
let private getContainer () =
    getService<CosmosClient>().GetContainer(shoppingDb, productsContainer)

let getById<'T> =
    getByIdAsync<'T> (getContainer ())

let getAll<'T> =
    getAllAsync<'T> (getContainer ())

let add<'T> =
    addAsync<'T> (getContainer ())

let update<'T> =
    updateAsync<'T> (getContainer ())
    
let delete<'T> =
    deleteAsync<'T> (getContainer ())
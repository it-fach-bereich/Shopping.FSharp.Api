module Shopping.Products.Domain

open Shopping.Models.Domain.Product
open Shopping.Data.Repository.Product
open Shopping.Files.Repository.Product

let getProductById id key = //: string -> obj -> Task<Result<Product,ReadItemFailureReason>> =
    getById<Product> id key

let getAllProducts () =
    getAll<Product>

let addProduct product =
    add<Product> product

let updateProduct product =
    update<Product> product

let deleteProduct id key =
    delete<Product> id key

let uploadProductFile file content =
    uploadFile file content

let downloadProductFile file =
    downloadFile file

let updateProductFile file content =
    updateFile file content

let deleteProductFile file =
    deleteFile file




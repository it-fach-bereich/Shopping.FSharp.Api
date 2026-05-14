module Shopping.Products.Domain

open System
open System.Linq.Expressions
open Shopping.Models.Domain.Product
open Shopping.Data.Repository.Product
open Shopping.Files.Repository.Product

let getProductById = getById

let getAllProducts = getAll

let searchProducts (search: ProductSearch) =
    let param = Expression.Parameter(typeof<Product>, "p")
    let containsMethod = typeof<string>.GetMethod("Contains", [| typeof<string> |])
    let conditions =
        [
            search.Category |> Option.map (fun v -> Expression.Equal(Expression.Property(param, "Category"),    Expression.Constant(v)) :> Expression)
            search.Name     |> Option.map (fun v -> Expression.Call(Expression.Property(param, "Name"),          containsMethod, Expression.Constant(v)) :> Expression)
            search.MinPrice |> Option.map (fun v -> Expression.GreaterThanOrEqual(Expression.Property(param, "Price"), Expression.Constant(v)) :> Expression)
            search.MaxPrice |> Option.map (fun v -> Expression.LessThanOrEqual(Expression.Property(param, "Price"),    Expression.Constant(v)) :> Expression)
            search.Clearance|> Option.map (fun v -> Expression.Equal(Expression.Property(param, "Clearance"),   Expression.Constant(v)) :> Expression)
        ]
        |> List.choose id
    let body =
        match conditions with
        | [] -> Expression.Constant(true) :> Expression
        | _  -> conditions |> List.reduce (fun acc expr -> Expression.AndAlso(acc, expr) :> Expression)
    let predicate = Expression.Lambda<Func<Product, bool>>(body, param)
    find predicate

let addProduct = add

let updateProduct = update

let deleteProduct = delete

let uploadProductFile = uploadFile

let downloadProductFile = downloadFile

let updateProductFile = updateFile

let deleteProductFile = deleteFile




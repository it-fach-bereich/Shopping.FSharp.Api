module Shopping.Customer.Domain

open System
open System.Linq.Expressions
open Shopping.Models.Domain.Customer
open Shopping.Data.Repository.Customer
open Shopping.Files.Repository.Customer

let getCustomerById = getById

let getAllCustomers = getAll

let searchCustomers (search: CustomerSearch) =
    let param = Expression.Parameter(typeof<Customer>, "c")
    let containsMethod = typeof<string>.GetMethod("Contains", [| typeof<string> |])
    let conditions =
        [
            search.FirstName  |> Option.map (fun v -> Expression.Call(Expression.Property(param, "FirstName"),  containsMethod, Expression.Constant(v)) :> Expression)
            search.LastName   |> Option.map (fun v -> Expression.Call(Expression.Property(param, "LastName"),   containsMethod, Expression.Constant(v)) :> Expression)
            search.Email      |> Option.map (fun v -> Expression.Call(Expression.Property(param, "Email"),       containsMethod, Expression.Constant(v)) :> Expression)
            search.PhoneNumber|> Option.map (fun v -> Expression.Call(Expression.Property(param, "PhoneNumber"), containsMethod, Expression.Constant(v)) :> Expression)
        ]
        |> List.choose id
    let body =
        match conditions with
        | [] -> Expression.Constant(true) :> Expression
        | _  -> conditions |> List.reduce (fun acc expr -> Expression.AndAlso(acc, expr) :> Expression)
    let predicate = Expression.Lambda<Func<Customer, bool>>(body, param)
    find predicate

let addCustomer = add

let updateCustomer = update

let deleteCustomer = delete

let uploadCustomerFile  = uploadFile

let downloadCustomerFile = downloadFile

let updateCustomerFile  = updateFile

let deleteCustomerFile  = deleteFile

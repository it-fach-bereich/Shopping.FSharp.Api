namespace Shopping.FSharp.Api.Controllers

open Microsoft.AspNetCore.Mvc
open Shopping.Common.Types
open Shopping.FSharp.Api.Dto
open Shopping.FSharp.Api.Dto.File
open Shopping.FSharp.Api.Dto.Customer
open Shopping.FSharp.Api.Dto.CustomerSearch
open Shopping.FSharp.Api.Mappers
open Shopping.Customer.Domain

[<ApiController>]
[<Route("api/[controller]")>]
type CustomersController () =
    inherit ShoppingControllerBase()


    [<HttpGet>]
    [<Route("")>]
    member this.GetAll() =
        task {
            let! result = getAllCustomers
            return
                match result with
                | Success x ->
                    let customers =
                        x
                        |> Seq.collect (fun feed -> feed :> seq<_>)
                        |> Seq.map CustomerMapper.toDto
                    this.Ok(customers) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpGet>]
    [<Route("{id}")>]
    member this.GetById(id: string) =
        task {
            let! result = getCustomerById id (StringKey id)
            return
                match result with
                | Success x -> this.Ok(CustomerMapper.toDto x) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("search")>]
    member this.Search([<FromBody>] search: CustomerSearchDto) =
        task {
            let! result = searchCustomers (CustomerMapper.toSearch search)
            return
                match result with
                | Success x -> this.Ok(x |> List.map CustomerMapper.toDto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("")>]
    member this.Add([<FromBody>] customer: CustomerDto) =
        task {
            let! result = addCustomer (CustomerMapper.toEntity customer)
            return
                match result with
                | Success x ->
                    let dto = CustomerMapper.toDto x
                    this.CreatedAtAction("GetById", {| id = dto.Id |}, dto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPut>]
    [<Route("")>]
    member this.Update([<FromBody>] customer: CustomerDto) =
        task {
            let! result = updateCustomer (CustomerMapper.toEntity customer)
            return
                match result with
                | Success _ -> this.NoContent() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpDelete>]
    [<Route("{id}")>]
    member this.Delete(id: string) =
        task {
            let! result = deleteCustomer id (StringKey id)
            return
                match result with
                | Success _ -> this.NoContent() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("file/upload")>]
    member this.UploadFile([<FromForm>] form: FileFormDto) =
        task {
            let metadata =
                Option.ofObj form.Metadata
                |> Option.map (fun metadata -> {
                    ProductId = metadata.ProductId
                    ProductCategory = metadata.ProductCategory
                })
            let fileDto = {
                FileName = form.FileName
                ContentType = form.ContentType
                Folder = form.Folder
                Size = form.File.Length
                Metadata = metadata
            }

            use stream = form.File.OpenReadStream()
            let! result = uploadCustomerFile (FileMapper.toEntity fileDto) stream
            return
                match result with
                | Success _ -> this.CreatedAtAction("DownloadFile", fileDto, fileDto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("file/download")>]
    member this.DownloadFile([<FromBody>] fileDto: FileDto) =
        task {
            let! result = downloadCustomerFile (FileMapper.toEntity fileDto)
            return
                match result with
                | Success stream -> this.File(stream, fileDto.ContentType, fileDto.FileName) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPut>]
    [<Route("file/update")>]
    member this.UpdateFile([<FromForm>] form: FileFormDto) =
        task {
            let metadata =
                Option.ofObj form.Metadata
                |> Option.map (fun metadata -> {
                    ProductId = metadata.ProductId
                    ProductCategory = metadata.ProductCategory
                })
            let fileDto = {
                FileName = form.FileName
                ContentType = form.ContentType
                Folder = form.Folder
                Size = form.File.Length
                Metadata = metadata
            }

            use stream = form.File.OpenReadStream()
            let! result = updateCustomerFile (FileMapper.toEntity fileDto) stream
            return
                match result with
                | Success _ -> this.Ok(fileDto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpDelete>]
    [<Route("file/delete")>]
    member this.DeleteFile([<FromBody>] fileDto: FileDto) =
        task {
            let! result = deleteCustomerFile (FileMapper.toEntity fileDto)
            return
                match result with
                | Success _ -> this.NoContent() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

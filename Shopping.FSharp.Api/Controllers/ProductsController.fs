namespace Shopping.FSharp.Api.Controllers

open Microsoft.AspNetCore.Mvc
open Shopping.Common.Types
open Shopping.FSharp.Api.Dto
open Shopping.Models.Dto.FileDto
open Shopping.Models.Dto.Product
open Shopping.Models.ProductMapper
open Shopping.Products.Domain

[<ApiController>]
[<Route("api/[controller]")>]
type ProductsController () =
    inherit ControllerBase()

    member private this.ToActionResult(failureReason: ReadItemFailureReason) : IActionResult =
        match failureReason with
        | DataNotFound s -> this.NotFound(s) :> IActionResult
        | BadRequest s -> this.BadRequest(s) :> IActionResult
        | Conflict s -> this.Conflict(s) :> IActionResult
        | Unauthorized s -> this.Unauthorized(s) :> IActionResult
        | Forbidden s -> this.Forbid(s) :> IActionResult
        | PreconditionFailed s -> this.StatusCode(412, s) :> IActionResult
        | TooManyRequests s -> this.StatusCode(429, s) :> IActionResult
        | ServiceUnavailable s -> this.StatusCode(503, s) :> IActionResult
        | ExceptionWithStatusCode (status, message) -> this.StatusCode(status, message) :> IActionResult
        | Exception s -> this.StatusCode(500, s) :> IActionResult

    [<HttpGet>]
    [<Route("")>]
    member this.GetAllProducts() =
        task {
            let! result = getAllProducts ()
            return
                match result with
                | Success x ->
                    let products =
                        x
                        |> Seq.collect (fun feed -> feed :> seq<_>)
                        |> Seq.map toDto
                    this.Ok(products) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpGet>]
    [<Route("{id}/{category}")>]
    member this.GetProductById(id: string, category: string) =
            task {
                let! result = getProductById id category
                return
                    match result with
                    | Success x -> this.Ok(toDto x) :> IActionResult
                    | Failure failureReason -> this.ToActionResult(failureReason)
            }

    [<HttpPost>]
    [<Route("")>]
    member this.AddProduct([<FromBody>] product: ProductDto) =
        task {
            let! result = addProduct (toEntity product)
            return
                match result with
                | Success x ->
                    let dto = toDto x
                    this.CreatedAtAction("GetProductById", {| id = dto.Id; category = dto.Category |}, dto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }
    
    [<HttpPut>]
    [<Route("")>]
    member this.UpdateProduct([<FromBody>] product: ProductDto) =
        task {
            let! result = updateProduct (toEntity product)
            return
                match result with
                | Success x -> this.Ok(toDto x) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }
    
    [<HttpDelete>]
    [<Route("{id}/{category}")>]
    member this.DeleteProduct(id: string, category: string) =
        task {
            let! result = deleteProduct id category
            return
                match result with
                | Success _ -> this.NoContent() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("file/upload")>]
    member this.UploadProductFile([<FromForm>] form: FileFormDto) =
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
            let! result = uploadProductFile fileDto stream
            return
                match result with
                | Success _ -> this.CreatedAtAction("DownloadProductFile", fileDto, fileDto) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPost>]
    [<Route("file/download")>]
    member this.DownloadProductFile([<FromBody>] fileDto: FileDto) =
        task {
            let! result = downloadProductFile fileDto
            return
                match result with
                | Success stream -> this.File(stream, fileDto.ContentType, fileDto.FileName) :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpPut>]
    [<Route("file/update")>]
    member this.UpdateProductFile([<FromForm>] form: FileFormDto) =
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
            let! result = updateProductFile fileDto stream
            return
                match result with
                | Success _ -> this.Ok() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

    [<HttpDelete>]
    [<Route("file/delete")>]
    member this.DeleteProductFile([<FromBody>] fileDto: FileDto) =
        task {
            let! result = deleteProductFile fileDto
            return
                match result with
                | Success _ -> this.NoContent() :> IActionResult
                | Failure failureReason -> this.ToActionResult(failureReason)
        }

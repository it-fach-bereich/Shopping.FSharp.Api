namespace Shopping.FSharp.Api.Controllers

open Microsoft.AspNetCore.Mvc
open Shopping.Common.Types

type ShoppingControllerBase() =
    inherit ControllerBase()

    [<NonAction>]
    member this.ToActionResult(failureReason: ReadItemFailureReason) : IActionResult =
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

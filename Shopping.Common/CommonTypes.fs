namespace Shopping.Common.Types

type Result<'T, 'TFAILURE> =
    | Success of 'T
    | Failure of 'TFAILURE
 
type UriType = UriType of string
type Email = Email of string

type Order =
    | Asc
    | Desc
type OrderFilter =
    { OrderBy:Order
      Field:string }
type OrderBy =
    | Asc of OrderFilter
    | Desc of OrderFilter
type SearchFilter =
    { Text:string
      Skip:int
      Limit:int
      OrderBy:OrderBy list }
type CosmosPartitionKey =
    | StringKey of string
    | IntKey of int

type ReadItemFailureReason =
    | DataNotFound of string
    | BadRequest of string
    | Conflict of string
    | Unauthorized of string
    | Forbidden of string
    | PreconditionFailed of string
    | TooManyRequests of string
    | ServiceUnavailable of string
    | ExceptionWithStatusCode of int * string
    | Exception of string
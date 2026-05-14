module Shopping.Models.Domain.Customer

open Newtonsoft.Json

type Customer = {
    [<JsonProperty("id")>]
    Id: string
    [<JsonProperty("firstName")>]
    FirstName: string
    [<JsonProperty("lastName")>]
    LastName: string
    [<JsonProperty("email")>]
    Email: string
    [<JsonProperty("phoneNumber")>]
    PhoneNumber: string
}

type CustomerSearch = {
    FirstName: string option
    LastName: string option
    Email: string option
    PhoneNumber: string option
}

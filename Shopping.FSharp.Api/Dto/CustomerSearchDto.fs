module Shopping.FSharp.Api.Dto.CustomerSearch

[<CLIMutable>]
type CustomerSearchDto = {
    FirstName: string
    LastName: string
    Email: string
    PhoneNumber: string
}

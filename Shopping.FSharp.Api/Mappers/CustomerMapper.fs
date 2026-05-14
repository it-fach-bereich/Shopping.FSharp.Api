module Shopping.FSharp.Api.Mappers.CustomerMapper

open Shopping.Models.Domain.Customer
open Shopping.FSharp.Api.Dto.Customer
open Shopping.FSharp.Api.Dto.CustomerSearch

let toDto (customer: Customer) : CustomerDto =
    {
        Id = customer.Id
        FirstName = customer.FirstName
        LastName = customer.LastName
        Email = customer.Email
        PhoneNumber = customer.PhoneNumber
    }

let toEntity (dto: CustomerDto) : Customer =
    {
        Id = dto.Id
        FirstName = dto.FirstName
        LastName = dto.LastName
        Email = dto.Email
        PhoneNumber = dto.PhoneNumber
    }

let toSearch (dto: CustomerSearchDto) : CustomerSearch =
    {
        FirstName   = Option.ofObj dto.FirstName
        LastName    = Option.ofObj dto.LastName
        Email       = Option.ofObj dto.Email
        PhoneNumber = Option.ofObj dto.PhoneNumber
    }

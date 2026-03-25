namespace Shopping.FSharp.Api
#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        builder.Services.AddSwaggerGen() |> ignore
        builder.Services.AddControllers() |> ignore

        let app = builder.Build()

        // Configure the HTTP request pipeline.
        if app.Environment.IsDevelopment() then
            app.UseSwagger() |> ignore

        app.UseSwaggerUI()
        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode

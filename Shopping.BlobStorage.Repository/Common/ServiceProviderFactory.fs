module Shopping.BlobStorage.Common.ServiceProviderFactory

open System
open Azure.Storage.Blobs
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

let private configuration =
    let environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional = false, reloadOnChange = true)
        .AddJsonFile($"appsettings.{environment}.json", optional = true, reloadOnChange = true)
        // .AddEnvironmentVariables()
        .Build()
        
let private initializedServiceProvider = lazy (
    let services = ServiceCollection()
    let blobConnectionString = configuration.GetValue<string>( "BlobStorage:ConnectionString" )
    let blobContainerName = configuration.GetValue<string>( "BlobStorage:Container" )
    let blobServiceClient = BlobServiceClient(connectionString = blobConnectionString)
    let blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName)
    blobContainerClient.CreateIfNotExists() |> ignore
    services.AddSingleton<BlobServiceClient>(blobServiceClient) |> ignore
    
    services.BuildServiceProvider())
    
let getService<'T>() = initializedServiceProvider.Value.GetService<'T>()
module Shopping.Data.Repository.Common.ServiceProviderFactory

open System
open Microsoft.Azure.Cosmos
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
    
    let cosmosEndpoint = configuration.GetValue<string>( "CosmosDb:Endpoint" )
    let cosmosKey = configuration.GetValue<string>( "CosmosDb:Key" )
    let cosmosDatabase = configuration.GetValue<string>( "CosmosDb:Database" )
    let cosmosContainer = configuration.GetValue<string>( "CosmosDb:ProductsContainer" )
    let cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey)
    cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDatabase)
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore
    let database = cosmosClient.GetDatabase(cosmosDatabase)
    database.CreateContainerIfNotExistsAsync(cosmosContainer, "/category")
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore
    database.CreateContainerIfNotExistsAsync("Customers", "/id")
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore
    database.CreateContainerIfNotExistsAsync("Orders", "/customerId")
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore
    services.AddSingleton<CosmosClient>(cosmosClient) |> ignore
    
    services.BuildServiceProvider())
    
let getService<'T>() = initializedServiceProvider.Value.GetService<'T>()
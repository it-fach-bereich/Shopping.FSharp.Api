module internal Shopping.Data.Repository.Common.Utils

open System
open System.Linq
open System.Linq.Expressions
open Microsoft.Azure.Cosmos
open Microsoft.Azure.Cosmos.Linq
open Shopping.Common.Types
open Newtonsoft.Json.Linq
open Shopping.Common.Utils

let private mapDataException (ex: CosmosException) =
    let statusCode = int ex.StatusCode
    match statusCode with
    | 404 -> DataNotFound "Requested data entity was not found."
    | 409 -> Conflict "A conflicting data entity already exists."
    | 400 -> BadRequest "The data request is invalid."
    | 401 -> Unauthorized "Access to data resources is unauthorized."
    | 403 -> Forbidden "Access to data resources is forbidden."
    | 412 -> PreconditionFailed "Data operation precondition failed."
    | 429 -> TooManyRequests "Too many data requests. Please retry later."
    | 503 -> ServiceUnavailable "Data service is temporarily unavailable."
    | _ -> ExceptionWithStatusCode (statusCode, "Data operation failed.")

let private getFeedResult<'T> (feed:FeedIterator<'T>) =
    task {
        match feed.HasMoreResults with
            | true -> 
                let! result = feed.ReadNextAsync()
                return [result]
            | false -> return []
    }

let internal getPartitionKey = function
    | StringKey s -> PartitionKey(partitionKeyValue = s)
    | IntKey i    -> PartitionKey(partitionKeyValue = i)

let getByIdAsync<'T> (container:Container) id partitionKey =
    task {
        try
            let! response = container.ReadItemAsync<'T>(id = id, partitionKey = getPartitionKey partitionKey)
            return Success response.Resource
        with
        | :? CosmosException as ex -> return Failure (mapDataException ex)
        | _ -> return Failure (Exception "Unexpected data operation error.")
    }

let getJObjectByIdAsync (container:Container) id partitionKey =
    task {
        try
            let! response = container.ReadItemAsync<JObject>(id = id, partitionKey = getPartitionKey partitionKey)
            return Success response
        with 
        | :? CosmosException as ex -> return Failure (mapDataException ex)
        | _ -> return Failure (Exception "Unexpected data operation error.")
    }
    
let getAllAsync<'T> (container:Container) =
    task {
        try
            let! result = getFeedResult (container.GetItemQueryIterator<'T>(queryText = "SELECT * FROM c"))
            return Success result
        with
        | :? CosmosException as ex -> return Failure (mapDataException ex)
        | _ -> return Failure (Exception "Unexpected data operation error.")
    }

let findAsync<'T> (container: Container) (predicate: Expression<Func<'T, bool>>) =
    task {
        try
            let iterator =
                container.GetItemLinqQueryable<'T>()
                    .Where(predicate)
                    .ToFeedIterator()
            let results = ResizeArray<'T>()
            while iterator.HasMoreResults do
                let! response = iterator.ReadNextAsync()
                results.AddRange(response)
            return Success (results |> Seq.toList)
        with
        | :? CosmosException as ex -> return Failure (mapDataException ex)
        | _ -> return Failure (Exception "Unexpected data operation error.")
    }

let addAsync<'T> (container:Container) entity =
    task {
        try
            let! response = container.CreateItemAsync<'T>(entity)
            return Success response.Resource
        with
        | :? CosmosException as ex -> return Failure (mapDataException ex)
        | _ -> return Failure (Exception "Unexpected data operation error.")
    }

let updateAsync<'T> (container:Container) entity=
     task {
         try
            let category = getPropertyValue<'T> "Category" entity |> string |> StringKey
            let! response = container.UpsertItemAsync<'T>(entity, getPartitionKey(category))
            return Success response.Resource
         with
         | :? CosmosException as ex -> return Failure (mapDataException ex)
         | _ -> return Failure (Exception "Unexpected data operation error.")
    }
  
let deleteAsync<'T> (container:Container) id partitionKey =
     task {
         try
            let! response = container.DeleteItemAsync<JObject>(id = id, partitionKey = getPartitionKey partitionKey)
            return Success response
         with
         | :? CosmosException as ex -> return Failure (mapDataException ex)
         | _ -> return Failure (Exception "Unexpected data operation error.")
    }

module Shopping.Common.Utils

open Microsoft.FSharp.Reflection

let getPropertyValue<'T> (propertyName: string) (record: 'T) =
    typeof<'T>
    |> FSharpType.GetRecordFields
    |> Array.find (fun p -> p.Name = propertyName)
    |> fun propInfo -> propInfo.GetValue(record)


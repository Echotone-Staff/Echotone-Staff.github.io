#r "nuget: FsHttp"

open FsHttp
open System.IO
open System.Text.Json

let swaggerUrl =
    "https://laab.ddns.net:38080/api/content/echotone/swagger/v1/swagger.json"

// remove recursively all properties called "nullable" from the swagger.json file
let rec removeNullables (json: JsonElement) =
    let removeNullable (json: JsonElement) =
        if json.ValueKind = JsonValueKind.Object then
            let properties = json.EnumerateObject()

            let newProperties =
                properties
                |> Seq.filter (fun prop -> (prop.Name <> "schemaName") && (prop.Name <> "nullable"))
                |> Seq.map (fun prop -> prop.Name, removeNullables prop.Value)
                |> dict

            let serialized = JsonSerializer.Serialize(newProperties)
            JsonDocument.Parse(serialized).RootElement
        else if json.ValueKind = JsonValueKind.Array then
            let items = json.EnumerateArray()
            let newItems = items |> Seq.map removeNullables |> Seq.toArray
            let serialized = JsonSerializer.Serialize(newItems)
            JsonDocument.Parse(serialized).RootElement
        else
            json

    removeNullable json


let options = JsonSerializerOptions(WriteIndented = true)
// Use utf8 encoding
options.Encoder <- System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping

let Serialize c = JsonSerializer.Serialize(c, options)

let Deserialize<'T> (c: string) =
    JsonSerializer.Deserialize<'T>(c, options)

// Get the content of swagger.json and remove every nullable property from it
http { GET swaggerUrl }
|> Request.send
|> Response.deserializeJson
|> removeNullables
|> Serialize
|> (fun c -> File.WriteAllText(__SOURCE_DIRECTORY__ + "/swagger.json", c))

__SOURCE_DIRECTORY__ + "/swagger20.json"
|> File.ReadAllText
|> Deserialize<JsonElement>
|> removeNullables
|> Serialize
|> (fun c -> File.WriteAllText(__SOURCE_DIRECTORY__ + "/swagger21.json", c))

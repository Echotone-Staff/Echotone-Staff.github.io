#r "nuget: Microsoft.OpenApi.Readers"
#r "nuget: Microsoft.OpenApi"

open Microsoft.OpenApi.Readers
open System.Net.Http
open Microsoft.OpenApi.Writers
open System.IO
open Microsoft.OpenApi.Validations

let (reader, _) =
    use httpClient =
        "https://laab.ddns.net:38080/api/content/echotone/"
        |> System.Uri
        |> (fun u -> new HttpClient(BaseAddress = u))

    let stream = httpClient.GetStreamAsync("swagger/v1/swagger.json").Result
    let settings = new OpenApiReaderSettings()
    settings.ReferenceResolution <- ReferenceResolutionSetting.ResolveAllReferences
    settings.LoadExternalRefs <- true
    (new OpenApiStreamReader()).Read(stream)

let outputSchemaV3 () =
    let filePath = __SOURCE_DIRECTORY__ + "/swagger30.json"
    use fileStream = File.Create(filePath)
    use streamWriter = new StreamWriter(fileStream)
    let jsonWriter = new OpenApiJsonWriter(streamWriter)
    reader.SerializeAsV3(jsonWriter)
    streamWriter.Flush() // Ensure all data is written to the file

outputSchemaV3 ()

let outputSchemaV2 () =
    let filePath = __SOURCE_DIRECTORY__ + "/swagger20.json"
    use fileStream = File.Create(filePath)
    use streamWriter = new StreamWriter(fileStream)
    let jsonWriter = new OpenApiJsonWriter(streamWriter)
    reader.SerializeAsV2(jsonWriter)
    streamWriter.Flush() // Ensure all data is written to the file

outputSchemaV2 ()

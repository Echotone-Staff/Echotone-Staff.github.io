#r "nuget: Squidex.ClientLibrary"
// #r "nuget: SwaggerProvider, 2.2.2"
#r "nuget: SwaggerProvider, 2.2.2"

open Squidex.ClientLibrary
open SwaggerProvider

[<Literal>]
let SchemaPath = __SOURCE_DIRECTORY__ + "/swagger30.json"

[<Literal>]
let ContentSchemaUrl =
    "https://laab.ddns.net:38080/api/content/echotone/swagger/v1/swagger.json"

// type EchotoneSchema = OpenApiClientProvider<SchemaPath>
type EchotoneSchema = OpenApiClientProvider<SchemaPath>

[<KeepCasing>]
type PageT = EchotoneSchema.PageDataDto

type PageData() =
    inherit Content<PageT>()

let clientOptions =
    SquidexOptions(
        Url = "https://laab.ddns.net:38080",
        AppName = "echotone",
        ClientId = "echotone:dev",
        ClientSecret = "mdb5fcui9hciepjrwimsoe3qtvcxc3zztzv7aidffsyx"
    )

let client = SquidexClient(clientOptions)

let pages = client.Contents<PageData, PageT>("page").GetAsync(ContentQuery()).Result
pages.Items |> Seq.iter (fun p -> printfn "%A" p.Data.Unit["fr"])
let pages = client.DynamicContents("page").GetAsync(ContentQuery()).Result

pages.Items
|> Seq.head
|> _.Data["unit"]
|> Seq.head
|> (fun u -> printfn "%A" (u.Children()))

let pageSchema =
    client.Schemas.GetSchemaAsync("page").Result.Fields
    |> Seq.iter (fun f -> printfn "%s" f.Name)

let oneUnit = pages |> Seq.head |> _.Data

oneUnit.GetType().GetProperties() |> Array.map (fun p -> p.Name)
oneUnit.Text

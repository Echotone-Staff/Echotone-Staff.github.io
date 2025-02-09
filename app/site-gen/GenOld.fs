namespace SiteGen

open SquidexApi
open System.Text.Json
open System.IO
open Site
open System.Net.Http
open System.Security.Cryptography
open System.Text
open System

module Gen =

    let sha256 = SHA256.Create()

    let toSha256Base64 (hashAlgo: SHA256) : string -> string =
        Encoding.UTF8.GetBytes >> hashAlgo.ComputeHash >> Convert.ToBase64String

    let hashAndConvertToBase64 = sha256 |> toSha256Base64

    let options = JsonSerializerOptions(WriteIndented = true)
    options.Encoder <- System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping

    let http = Config.fsReadyHttp ()
    let client = SquidexApiGraphqlClient(httpClient)
    let config = Config.getConfig ()
    let sourceDir = Path.Combine(__SOURCE_DIRECTORY__, "..", "..")
    let assetsDir = Path.Combine(sourceDir, config["ASSETS_DIR"])
    let thumbDir = Path.Combine(assetsDir, "thumbnails")
    let dataDir = Path.Combine(sourceDir, config["DATA_DIR"])
    let gqlDir = Path.Combine(sourceDir, config["GQL_DIR"])

    let checkFile (asset: Assets.Asset) =
        if not (Directory.Exists(assetsDir)) then
            Directory.CreateDirectory(assetsDir) |> ignore

        let path = Path.Combine(assetsDir, asset.slug)

        if File.Exists(path) then
            let fileStream = File.OpenRead(path)
            let hashStream = Convert.ToBase64String(sha256.ComputeHash(fileStream))

            asset.fileHash = hashAndConvertToBase64 $"{hashStream}{asset.fileName}{asset.fileSize}"
        else
            false

    let refreshAsset (client: HttpClient) (asset: Assets.Asset) =

        printfn "Checking %s" asset.fileName
        let path = Path.Combine(assetsDir, asset.slug)

        if not (checkFile asset) then
            printfn "Downloading %s" asset.slug
            let bytes = client.GetByteArrayAsync(asset.url).Result
            File.WriteAllBytes(path, bytes)
            // Download thumbnail if present
            match asset.thumbnailUrl with
            | Some url ->
                let thumbnailPath = Path.Combine(thumbDir, asset.slug)

                if not (Directory.Exists(thumbDir)) then
                    Directory.CreateDirectory(thumbDir) |> ignore

                let thumbnailBytes = client.GetByteArrayAsync(url).Result
                File.WriteAllBytes(thumbnailPath, thumbnailBytes)
            | None -> ()

    let refreshAssets () =

        let assets = client.Assets()

        match assets with
        | Ok result ->
            let assets = result.queryAssets

            for asset in assets do
                refreshAsset httpClient asset
        | Error e -> printfn "%A" e

    let justDumpJson file o =
        let json = JsonSerializer.Serialize(o, options)

        if (not (Directory.Exists(dataDir))) then
            Directory.CreateDirectory(dataDir) |> ignore

        File.WriteAllText(Path.Combine(dataDir, file), json)

    // let dumpQuery file query =
    //     match query with
    //     | Ok result -> result.ToString() |> justDumpJson file
    //     | Error e -> printfn "%A" e

    let dumpQuery (path: string) =
        let file = Path.GetFileNameWithoutExtension(path)

        path
        |> File.ReadAllText
        |> (fun q -> httpClient.PostAsync("graphql", new StringContent(q, Encoding.UTF8, "application/json")))
        |> (fun r -> r.Result.Content.ReadAsStringAsync().Result)
        |> (fun r -> File.WriteAllText(Path.Combine(dataDir, file) + ".json", r))

    let dumpSite () =
        Directory.GetFiles(gqlDir) |> Array.map dumpQuery |> ignore

// dumpQuery "pages.json" (client.Pages())
// dumpQuery "posts.json" (client.Aboutcontact())
// dumpQuery "assets.json" (client.Assets())

namespace Site

module Config =
    open FsHttp
    open System.IO
    open System.Net.Http
    open dotenv.net
    open System.Net

    let sourceDir = Path.Combine(__SOURCE_DIRECTORY__, "..", "..")

    let getConfig () =
        DotEnv.Read(DotEnvOptions(envFilePaths = [ Path.Combine(sourceDir, ".env-app.local") ]))

    let loadSecret () =
        if File.Exists(__SOURCE_DIRECTORY__ + "/../../.env-secret.local") then
            DotEnv.Load(DotEnvOptions(envFilePaths = [ __SOURCE_DIRECTORY__ + "/../../.env-secret.local" ]))

        System.Environment.GetEnvironmentVariable

    let getToken () = DotEnv.Read()["TOKEN"]

    let refresh () =
        let envVars = getConfig ()

        let token =
            http {
                config_useBaseUrl envVars["BASE_URL"]
                POST "/identity-server/connect/token"
                body

                formUrlEncoded
                    [ "grant_type", "client_credentials"
                      "client_id", envVars["CLIENT_ID"]
                      "client_secret", envVars["CLIENT_SECRET"] ]
            }
            |> Request.send
            |> Response.deserializeJson
            |> (fun x -> (x?access_token).GetString())

        File.WriteAllText(Path.Combine(sourceDir, ".env"), $"TOKEN={token}")
        let baseUrl = envVars["BASE_URL"]

        $"""schema: "app/schema.json"
documents: "app/queries/*.gql"
extensions:
    endpoints:
        default:
            url: "{baseUrl}"
            headers:
                Authorization: "Bearer {token}"
generates:
    app/gql.tsx:
        plugins:
            - typescript
"""
        |> (fun t -> File.WriteAllText(__SOURCE_DIRECTORY__ + "/../../graphql.config.yml", t))

    let fsReadyHttp () =
        let envVars = getConfig ()
        let token = getToken ()

        http {
            config_useBaseUrl envVars["BASE_URL"]
            AuthorizationBearer token
        }

    let validateOrRefresh () =
        let appName = getConfig()["APP_NAME"]

        fsReadyHttp () { GET $"/api/apps/{appName}/assets" }
        |> Request.send
        |> function
            | r when r.statusCode = HttpStatusCode.OK ->
                printfn "Token OK"
                ()
            | _ ->
                printfn "Token invalid, refreshing..."
                refresh ()

    let refreshSchema () =
        let appName = getConfig()["APP_NAME"]

        let introspectionText =
            File.ReadAllBytes(__SOURCE_DIRECTORY__ + "/../introspection.gql")

        fsReadyHttp () {
            POST $"/api/{appName}/graphql"

            body
            ContentType "application/graphql"
            // jsonSerialize {| query = introspectionText |}
            binary introspectionText
        }

        |> Request.send
        |> Response.toText
        |> (fun r -> File.WriteAllText(__SOURCE_DIRECTORY__ + "/../schema.json", r))

    let refreshJsons () =
        let config = getConfig ()
        let appName = config["APP_NAME"]
        let dataDir = config["DATA_DIR"]

        let siteParts =
            Map
                [ "assets", $"/api/apps/{appName}/assets"
                  "about", $"/api/content/{appName}/about"
                  "contact", $"/api/content/{appName}/contact"
                  "pages", $"/api/content/{appName}/page" ]

        if not (Directory.Exists(dataDir)) then
            Directory.CreateDirectory(dataDir) |> ignore

        siteParts
        |> Map.iter (fun key value ->
            fsReadyHttp () { GET value }
            |> Request.send
            |> Response.saveFile (Path.Combine(dataDir, $"{key}.json")))

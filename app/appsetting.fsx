#r "nuget: FsHttp"
#r "nuget: dotenv.net"


open FsHttp
open System.IO

open dotenv.net

let envVars =
    DotEnv.Read(DotEnvOptions(envFilePaths = [ __SOURCE_DIRECTORY__ + "/../.echotone.env" ]))

let token =
    http {
        POST envVars["TOKEN_URL"]
        body

        formUrlEncoded
            [ "grant_type", "client_credentials"
              "client_id", envVars["CLIENT_ID"]
              "client_secret", envVars["CLIENT_SECRET"] ]
    }
    |> Request.send
    |> Response.deserializeJson
    |> (fun x -> (x?access_token).GetString())

File.WriteAllText(__SOURCE_DIRECTORY__ + "/../.env", $"ECHOTONE_TOKEN={token}")
let baseUrl = envVars["BASE_URL"]

$"""schema: "app/schema.json"
documents: "app/queries/*.gql"
extensions:
  endpoints:
    default:
      url: "{baseUrl}"
      headers:
        Authorization: "Bearer {token}"
"""
|> (fun t -> File.WriteAllText(__SOURCE_DIRECTORY__ + "/../graphql.config.yml", t))

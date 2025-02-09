#r "nuget: FsHttp"
#r "nuget: dotenv.net"

open FsHttp
open System.IO
open dotenv.net

let tokenVars = DotEnv.Read()
let token = tokenVars["ECHOTONE_TOKEN"]

let envVars =
    DotEnv.Read(DotEnvOptions(envFilePaths = [ __SOURCE_DIRECTORY__ + "/../.echotone.env" ]))

let introspectionText =
    File.ReadAllBytes(__SOURCE_DIRECTORY__ + "/introspection.gql")

http {
    POST envVars["BASE_URL"]
    headers [ "Authorization", $"bearer {token}" ]
    body
    ContentType "application/graphql"
    // jsonSerialize {| query = introspectionText |}
    binary introspectionText
}
|> Request.send
|> Response.toText
|> (fun r -> File.WriteAllText(__SOURCE_DIRECTORY__ + "/schema.json", r))

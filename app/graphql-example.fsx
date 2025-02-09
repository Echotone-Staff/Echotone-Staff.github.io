#r "nuget: FSharp.Data.GraphQL.Client"
#r "nuget: FsHttp"

open FSharp.Data.GraphQL

open FsHttp

[<Literal>]
let appUrl = "https://laab.ddns.net:38080/api/content/echotone/graphql"

let tokenUrl = "https://laab.ddns.net:38080/identity-server/connect/token"
let clientSecret = "mdb5fcui9hciepjrwimsoe3qtvcxc3zztzv7aidffsyx"

let token =
    http {
        POST tokenUrl
        body

        formUrlEncoded
            [ "grant_type", "client_credentials"
              "client_id", "echotone:dev"
              "client_secret", clientSecret ]
    }
    |> Request.send
    |> Response.deserializeJson
    |> (fun x -> (x?access_token).GetString())

let run () =
    // Dispose the connection after using it.
    use connection = new GraphQLClientConnection()

    let request: GraphQLRequest =
        { Query =
            """query q {
	queryPageContents {
    id
    data {
      unit {
        fr {
          title
          short
          text
        }
      }
    }
  }
}"""
          Variables = [||]
          ServerUrl = appUrl
          HttpHeaders = [| "Authorization", $"bearer {token}" |]
          OperationName = Some "q" }

    let response = GraphQLClient.sendRequest connection request
    printfn "%s" (response.Content.ReadAsStringAsync().Result)

run ()

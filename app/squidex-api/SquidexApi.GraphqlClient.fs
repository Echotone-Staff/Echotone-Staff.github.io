namespace SquidexApi

open System
open System.IO
open System.Net.Http
open System.Net.Http.Json
open System.Text
open System.Text.Json
open System.Text.Json.Serialization

type GraphqlInput<'T> = { query: string; variables: Option<'T> }
type GraphqlSuccessResponse<'T> = { data: 'T }
type GraphqlErrorResponse = { errors: ErrorType list }

type SquidexApiGraphqlClient private (url: string, options: JsonSerializerOptions, httpClient: HttpClient) =
    static let defaultOptions : JsonSerializerOptions =
        let options = JsonSerializerOptions ()
        let encoding =
            JsonUnionEncoding.InternalTag
            ||| JsonUnionEncoding.UnwrapRecordCases
            ||| JsonUnionEncoding.UnwrapFieldlessTags
            ||| JsonUnionEncoding.UnwrapOption
            ||| JsonUnionEncoding.AdjacentTag
        let converter = JsonFSharpConverter(encoding, unionTagName="__typename")
        options.Converters.Add(converter)
        options

    /// <summary>Creates SquidexApiGraphqlClient specifying <see href="T:System.Net.Http.HttpClient">HttpClient</see> instance</summary>
    /// <param name="url">GraphQL endpoint URL</param>
    /// <param name="httpClient">The HttpClient to use for issuing the HTTP requests</param>
    /// <param name="options">The JSON serialization options</param>
    new(url: string, httpClient: HttpClient, options: JsonSerializerOptions) =
        SquidexApiGraphqlClient(url, options, httpClient)

    /// <summary>Creates SquidexApiGraphqlClient</summary>
    /// <param name="url">GraphQL endpoint URL</param>
    /// <param name="options">The JSON serialization options</param>
    new(url: string, options: JsonSerializerOptions) = SquidexApiGraphqlClient(url, new HttpClient(), defaultOptions)

    /// <summary>Creates SquidexApiGraphqlClient specifying <see href="T:System.Net.Http.HttpClient">HttpClient</see> instance</summary>
    /// <param name="url">GraphQL endpoint URL</param>
    /// <param name="httpClient">The HttpClient to use for issuing the HTTP requests</param>
    new(url: string, httpClient: HttpClient) = SquidexApiGraphqlClient(url, httpClient, defaultOptions)

    /// <summary>Creates SquidexApiGraphqlClient</summary>
    /// <param name="url">GraphQL endpoint URL</param>
    new(url: string) =
        SquidexApiGraphqlClient(url, new HttpClient(), defaultOptions)

    /// <summary>Creates SquidexApiGraphqlClient specifying <see href="T:System.Text.Json.Serialization.JsonFSharpOptions">JsonFSharpOptions</see> instance</summary>
    /// <param name="url">GraphQL endpoint URL</param>
    /// <param name="fsOptions">The JSON serialization options</param>
    new(url: string, fsOptions: JsonFSharpOptions) =
        let options = defaultOptions
        options.Converters.Add (JsonFSharpConverter(fsOptions))
        SquidexApiGraphqlClient(url, new HttpClient(), options)

    /// <summary>Creates SquidexApiGraphqlClient specifying <see href="T:System.Net.Http.HttpClient">HttpClient</see> instance</summary>
    new(httpClient: HttpClient, options: JsonSerializerOptions) =
        if httpClient <> null then
            if httpClient.BaseAddress <> null then
                SquidexApiGraphqlClient(httpClient.BaseAddress.OriginalString, httpClient, options)
            else
                raise(ArgumentNullException("BaseAddress of the HttpClient cannot be null for that constructor that only accepts HttpClient without the url parameter"))
                SquidexApiGraphqlClient(String.Empty, httpClient, options)
        else
            raise(ArgumentNullException("The input HttpClient cannot be null for constructor of SquidexApiGraphqlClient"))
            SquidexApiGraphqlClient(String.Empty, httpClient, defaultOptions)

    /// <summary>Creates SquidexApiGraphqlClient specifying <see href="T:System.Net.Http.HttpClient">HttpClient</see> instance</summary>
    /// <param name="httpClient"><see href="HttpClient">HttpClient</see> instance with
    /// <strong>BaseAddress set to GraphQL endpoint URL</strong></param>
    /// <exception cref="T:System.ArgumentNullException">when httpClient parameter is null</exception>
    /// <exception cref="T:System.ArgumentNullException">when httpClient.<see href="P:HttpClient.BaseAddress">BaseAddress</see> property is null</exception>
    new(httpClient: HttpClient) =
        if httpClient <> null then
            if httpClient.BaseAddress <> null then
                SquidexApiGraphqlClient(httpClient.BaseAddress.OriginalString, httpClient, defaultOptions)
            else
                raise(ArgumentNullException("BaseAddress cannot be null for constructor without the url parameter"))
                SquidexApiGraphqlClient(String.Empty, httpClient, defaultOptions)
        else
            raise(ArgumentNullException("The input HttpClient cannot be null for constructor of SquidexApiGraphqlClient"))
            SquidexApiGraphqlClient(String.Empty, httpClient, defaultOptions)

    member _.AboutcontactAsync() =
        async {
            let query = """
                query aboutcontact {
                  findAboutSingleton {
                    data {
                      content {
                        fr {
                          title
                          short
                          text
                        }
                      }
                    }
                  },
                  findContactSingleton {
                    data {
                      content {
                        fr {
                          title
                          short
                          text
                          locations {
                            location
                          }
                        }
                      }
                    }
                  }
                }
            """

            
            let! response =
                httpClient.PostAsJsonAsync(url, { query = query; variables = None }, options)
                |> Async.AwaitTask
            let! responseContent = response.Content.ReadAsStreamAsync() |> Async.AwaitTask
            let! responseJson = JsonSerializer.DeserializeAsync<JsonElement>(responseContent, options).AsTask() |> Async.AwaitTask
            responseContent.Seek(0L, SeekOrigin.Begin) |> ignore

            match response.IsSuccessStatusCode with
            | true ->
                let errorsReturned =
                    match responseJson.TryGetProperty ("errors") with
                    | true, value -> value.GetArrayLength() > 0
                    | false, _ -> false

                if errorsReturned then
                    let! response = JsonSerializer.DeserializeAsync<GraphqlErrorResponse>(responseContent, options).AsTask() |> Async.AwaitTask
                    return Error response.errors
                else
                    let! response = JsonSerializer.DeserializeAsync<GraphqlSuccessResponse<Aboutcontact.Query>>(responseContent, options).AsTask() |> Async.AwaitTask
                    return Ok response.data

            | errorStatus ->
                let! response = JsonSerializer.DeserializeAsync<GraphqlErrorResponse>(responseContent, options).AsTask() |> Async.AwaitTask
                return Error response.errors
        }

    member this.Aboutcontact() = Async.RunSynchronously(this.AboutcontactAsync())


    member _.AssetsAsync() =
        async {
            let query = """
                query Assets {
                  queryAssets {
                    id
                    thumbnailUrl
                    fileName
                    fileHash
                    fileType
                    fileSize
                    slug
                    tags
                    sourceUrl
                    lastModified
                    url
                  }
                }
            """

            
            let! response =
                httpClient.PostAsJsonAsync(url, { query = query; variables = None }, options)
                |> Async.AwaitTask
            let! responseContent = response.Content.ReadAsStreamAsync() |> Async.AwaitTask
            let! responseJson = JsonSerializer.DeserializeAsync<JsonElement>(responseContent, options).AsTask() |> Async.AwaitTask
            responseContent.Seek(0L, SeekOrigin.Begin) |> ignore

            match response.IsSuccessStatusCode with
            | true ->
                let errorsReturned =
                    match responseJson.TryGetProperty ("errors") with
                    | true, value -> value.GetArrayLength() > 0
                    | false, _ -> false

                if errorsReturned then
                    let! response = JsonSerializer.DeserializeAsync<GraphqlErrorResponse>(responseContent, options).AsTask() |> Async.AwaitTask
                    return Error response.errors
                else
                    let! response = JsonSerializer.DeserializeAsync<GraphqlSuccessResponse<Assets.Query>>(responseContent, options).AsTask() |> Async.AwaitTask
                    return Ok response.data

            | errorStatus ->
                let! response = JsonSerializer.DeserializeAsync<GraphqlErrorResponse>(responseContent, options).AsTask() |> Async.AwaitTask
                return Error response.errors
        }

    member this.Assets() = Async.RunSynchronously(this.AssetsAsync())


    member _.PagesAsync() =
        async {
            let query = """
                query pages {
                queryPageContents {
                  data {
                    unit {
                      fr {
                        title
                        short
                        text
                      }
                    }
                    medias {
                      iv {
                        medias {
                          url
                          thumbnailUrl
                          fileType
                          fileName
                          fileHash
                        }
                      }
                    }
                  }
                }
                }
            """

            
            let! response =
                httpClient.PostAsJsonAsync(url, { query = query; variables = None }, options)
                |> Async.AwaitTask
            let! responseContent = response.Content.ReadAsStreamAsync() |> Async.AwaitTask
            let! responseJson = JsonSerializer.DeserializeAsync<JsonElement>(responseContent, options).AsTask() |> Async.AwaitTask
            responseContent.Seek(0L, SeekOrigin.Begin) |> ignore

            match response.IsSuccessStatusCode with
            | true ->
                let errorsReturned =
                    match responseJson.TryGetProperty ("errors") with
                    | true, value -> value.GetArrayLength() > 0
                    | false, _ -> false

                if errorsReturned then
                    let! response = JsonSerializer.DeserializeAsync<GraphqlErrorResponse>(responseContent, options).AsTask() |> Async.AwaitTask
                    return Error response.errors
                else
                    let! response = JsonSerializer.DeserializeAsync<GraphqlSuccessResponse<Pages.Query>>(responseContent, options).AsTask() |> Async.AwaitTask
                    return Ok response.data

            | errorStatus ->
                let! response = JsonSerializer.DeserializeAsync<GraphqlErrorResponse>(responseContent, options).AsTask() |> Async.AwaitTask
                return Error response.errors
        }

    member this.Pages() = Async.RunSynchronously(this.PagesAsync())

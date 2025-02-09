open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.JavaScript
open Site.Config
open FsHttp
open System.Net


let initTargets () =
    Target.create "validate" (fun _ -> validateOrRefresh ())

    Target.create "refresh" (fun _ -> refresh ())

    Target.create "refreshJsons" (fun _ -> refreshJsons ())

    Target.create "buildFS" (fun _ ->
        let result = DotNet.exec id "build" "echotone-web.sln -c Release"

        if not result.OK then
            failwithf "build.fs failed with code %i" result.ExitCode)



    // Target.create "snowflaqe" (fun _ ->
    //     let result = DotNet.exec id "snowflaqe" "--generate"

    //     if not result.OK then
    //         failwithf "Snowflaqe failed with code %i" result.ExitCode)

    // Target.create "graphqlCodeGen" (fun _ -> Npm.run "codegen" id)

    // "validate" ==> "refreshSchema" ==> "snowflaqe" ==> "graphqlCodeGen"
    "validate" ==> "refreshJsons" ==> "buildFS"

let runOrDefault args =
    let execContext = Context.FakeExecutionContext.Create false "build.fsx" []
    Context.setExecutionContext (Context.RuntimeContext.Fake execContext)
    initTargets () |> ignore

    try
        match args with
        | [| target |] -> Target.runOrDefault target
        | _ -> Target.runOrDefault "validate"

        0
    with e ->
        printfn "%A" e
        1

[<EntryPoint>]
let main args = runOrDefault args

module Server

open Saturn
open Giraffe
open System.IO

open Giraffe.Core
open Giraffe.ResponseWriters
open WriteFile
open FSharp.Control.Tasks.V2.ContextInsensitive
open System

type WebSocketServerMessage = { Time : System.DateTime; Message : string }

let saveRcoFileAsync init = task {
    update init WriteFile.Initialize
}


let defaultView = router {
    post "/api/SaveRCOList" (fun next ctx ->
        task{
            let! init = ctx.BindModelAsync<WriteFile.Model>()
            do! saveRcoFileAsync init
            return! (next ctx)
        })
    get "/api/Test" (fun next ctx ->
        task{
            let testobj = {Time = System.DateTime.Now ; Message = "Current time ya'll"}
            return! json testobj next ctx
        })
}

let app =
    application {
        url "http://localhost:8085"
        use_router defaultView
        memory_cache
        use_json_serializer(Thoth.Json.Giraffe.ThothSerializer())
        use_static "public"
        use_gzip
    }

run app

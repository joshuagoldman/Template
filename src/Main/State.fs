module Main.State

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open Browser.Dom
open Types
open JsInterop
open Global.Types


let init =
    {
        Name = ""
    }


let update msg (model:Model) : Model * GlobalMsg * Cmd<Msg> =
    match msg with
    | Batch_Main msgs ->
        let batchedMsgs =
            msgs
            |> Array.map (fun msg -> msg |> Cmd.ofMsg)
            |> Cmd.batch
        model, MsgNone, batchedMsgs
    | MsgNone_Main ->
        model, MsgNone,[]
    | GlobalMsg_Main global_msg ->
        model, global_msg,[]
    | Popup_Msg style ->
        let msg =
            style
            |> Global.Types.Popup_Msg_Global

        model, msg, []
        
    




module Main.Logic

open JsInterop
open Elmish
open Fable.Import
open System
open Types
open Global.Types
open Feliz

let standardPositions = {
        Popup.Types.PosX = 250.0
        Popup.Types.PosY = 250.0
    }

let QuestionPopup msg yesMsg noMsg dispatch positions =

    let questionToAsk =
        msg
        |> Popup.View.getPopupMsg

    let yesNoButtons =
        Popup.View.yesNoButtons yesMsg noMsg dispatch
        |> List.toArray

    let popupStyle =
        (yesNoButtons,questionToAsk)
        |> Popup.Types.Alternative_Popup_Otpions.Several_Alternatives

    (popupStyle,positions) |>
    (
        Popup.Types.PopupStyle.Has_Alternatives >>
        Global.Types.Popup_Msg_Global >>
        delayedMessage 3000
    )

let errorPopupMsg dispatch msgToDispatch positions msg =
    let reactElementMsgs =
        msg
        |> Popup.View.getPopupMsg

    let simpleOkButton =
        Popup.View.simpleOkButton msgToDispatch dispatch

    let allComponentsSimleOk =
        (simpleOkButton,reactElementMsgs)
        |> Popup.Types.Alternative_Popup_Otpions.Simple_Ok

    (allComponentsSimleOk,positions) |>
    (
        Popup.Types.PopupStyle.Has_Alternatives >>
        Main.Types.Popup_Msg
    )

let checkingProcessPopupMsg positions msg =

    (msg,positions) |>
    (
        Popup.Types.PopupStyle.Has_No_Alternatives >>
        Main.Types.Popup_Msg 
    )

let killPopupMsg =
    Popup.Types.PopupStyle.Popup_Is_Dead |>
    (
        Main.Types.Popup_Msg
    )

let contentChanged ( model : Main.Types.Model ) new_content =
    let new_model = {model with CurrContent = new_content |> Yes_Defined}

    new_model

let write2File ( dispatch : Types.Msg -> unit ) content popupPosition = async {

    let fData = Browser.FormData.Create()

    let startSaveMsg = "Starting save process..."

    let popupInfoStr =
        0.0 |>
        (
            Popup.View.getPopupMsgProgress startSaveMsg >>
            checkingProcessPopupMsg popupPosition
        )

    let path_2_executable = "cd server;scripts/WriteFile"
    let download_rate = "3"

    let shellcommands = 
        String.Format (
            "{0};{1};{2}",
            path_2_executable,
            download_rate,
            content
        )

    popupInfoStr |> dispatch

    fData.append("shellCommand", shellcommands)

    let request =
        Async.FromContinuations <| fun (resolve,_,_) ->

            let xhr = Browser.XMLHttpRequest.Create()
            xhr.``open``(method = "POST", url = "http://localhost:3001/shellcommand")
            xhr.timeout <- 10000.0

            let socketResponse = NetSocket.connect "localhost" 300
    
            match socketResponse.ErrorMessage with
            | None  ->
                socketResponse.Socket.Value
                |> NetSocket.listen (fun scktMsg ->
                    let eventResult = (scktMsg :?> string)

                    match eventResult.ToLower().Contains("Finished") with
                    | true ->
                        let msg = "loading file (" + eventResult + "% loaded)"

                        let popupInfoStr =
                            eventResult |>
                            (
                                float >>
                                Popup.View.getPopupMsgProgress msg >>
                                checkingProcessPopupMsg popupPosition >>
                                dispatch
                            )

                        popupInfoStr
                    | _ ->
                        resolve
                            {
                                Status = 404
                                Msg = "Finished!"
                            }
                        
                            
                    )
                |> ignore

            | Some error ->

                resolve
                    {
                        Status = 404
                        Msg = error
                    }

            xhr.ontimeout <- fun _ ->
                let error =
                    "Connection timed out."

                resolve
                    {
                        Status = 404
                        Msg = error
                    }

            xhr.send(fData) |> fun  _ -> ()

    let! response = request

    let popup_msg = errorPopupMsg
                            dispatch
                            killPopupMsg
                            popupPosition
                            response.Msg
                        
    popup_msg |> dispatch
}
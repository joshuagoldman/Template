module Global.Types

open Fable.React
open Fable.Core.JsInterop
open Fable.Import

type AsynSyncMix<'a> =
    | Is_Async of Async<'a>
    | Is_Not_Async of 'a

type Loading_Popup_Options =
    | Spinner_Popup
    | Progress_Popup of float
    | No_Loading_Popup_Type

let sleepAsync time = async {
    do! Async.Sleep time
}

let getPositions ev =
    {
        Popup.Types.PosX = ( ev?pageX : float )
        Popup.Types.PosY = ( ev?pageY : float )
    }

let delayedMessage time ( msg : 'msg ) =
    async{
        if time < 30000 && time > 0
        then do! Async.Sleep time
        else do! Async.Sleep 0

        return(msg)
    }

let request ( data : obj ) = 
    Async.FromContinuations <| fun (resolve,_,_) ->
        let xhr = Browser.XMLHttpRequest.Create()
        xhr.``open``(method = "POST", url = "http://localhost:3001/shellcommand")
        xhr.setRequestHeader("Content-Type","application/x-www-form-urlencoded")
        

        xhr.onreadystatechange <- fun _ ->
            if xhr.readyState = (4 |> float)
            then
                resolve(xhr)

        xhr.send(data)

let simpleGetRequest urlstr = 
    Async.FromContinuations <| fun (resolve,_,_) ->
        let xhr = Browser.XMLHttpRequest.Create()
        xhr.``open``(method = "GET", url = urlstr)

        xhr.onreadystatechange <- fun _ ->
            if xhr.readyState = (4 |> float)
            then
                resolve(xhr)

        xhr.send(None)

let requestCustom url ( data : obj ) = 
    Async.FromContinuations <| fun (resolve,_,_) ->
        let xhr = Browser.XMLHttpRequest.Create()
        xhr.``open``(method = "POST", url = url)
        xhr.setRequestHeader("Content-Type","application/x-www-form-urlencoded")
        

        xhr.onreadystatechange <- fun _ ->
            if xhr.readyState = (4 |> float)
            then
                resolve(xhr)

        xhr.send(data)

type HttpResponse =
    | TimedOut of string
    | Success of Browser.XMLHttpRequest

type FinishedType = {
    Status : int
    Msg : string
}

type MessageTypeID = {
    Progress : float
    Remaining : int
    ID : string
}

type FinishedTypeID = {
    Status : int
    Msg : string
    ID : string
}

let requestFormDataStyle ( fData : Browser.FormData ) url =
    Async.FromContinuations <| fun (resolve,_,_) ->

        let xhr = Browser.XMLHttpRequest.Create()
        xhr.``open``(method = "POST", url = url)
        xhr.timeout <- 8000.0
    

        xhr.onreadystatechange <- fun _ ->
            if xhr.readyState = (4 |> float)
            then
                resolve (xhr |> HttpResponse.Success)

        xhr.ontimeout <-fun _ ->
            let timeoutStr =
                "Connection timed out."
            resolve (timeoutStr |> HttpResponse.TimedOut)

        xhr.send(fData)

type GlobalMsg =
    | MsgNone
    | Popup_Msg_Global of Popup.Types.PopupStyle
    | Batch of GlobalMsg[]




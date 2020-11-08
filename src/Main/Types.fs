module Main.Types

open Global.Types

type GitLog = {
    Message : string
    Commit : string
    Date : string
}

type GitBranch = {
    Name : string
    Log : GitLog []
}

type Msg =
    | Batch_Main of Msg []
    | MsgNone_Main
    | GlobalMsg_Main of GlobalMsg
    | Popup_Msg of Popup.Types.PopupStyle


type Model = {
    Name : string
}


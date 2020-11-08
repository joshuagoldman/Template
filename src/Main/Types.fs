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

type Defined<'a> = 
    | Yes_Defined of 'a
    | Not_Defined

type Msg =
    | Batch_Main of Msg []
    | MsgNone_Main
    | GlobalMsg_Main of GlobalMsg
    | Popup_Msg of Popup.Types.PopupStyle
    | Change_File_Content of Browser.Types.Event
    | Write_To_File of (Msg -> unit) * Popup.Types.PopupPosition


type Model = {
    CurrContent : Defined<string>
}


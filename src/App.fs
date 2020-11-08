module App.View

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open Fable.Core.JsInterop
open Types
open App.State
open Feliz
open Feliz.style
open Fable.React

importAll "../sass/main.sass"

let root (model: Model) dispatch =
    Html.div[
        Main.View.root model.Main (Types.MainMsg >> dispatch)
    ]

open Elmish.React

// App
Program.mkProgram init update root
|> Program.withReactBatched "elmish-app"
|> Program.run

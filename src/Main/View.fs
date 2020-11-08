module Main.View

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open Fable.Core.JsInterop
open Types
open Feliz
open Feliz.style

open Fable.React
open Fable.React.Props

let root ( model : Model ) dispatch =
    Html.div[
        prop.text "Welcome to this Template yo!"
    ]

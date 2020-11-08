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

let write2FileButton model dispatch =
    match model.CurrContent with
    | Yes_Defined _ ->
        Html.div[
            prop.className "button"
            prop.text "Write content to file"
            prop.style[
                Feliz.style.fontSize 17
                fontWeight.bold
                Feliz.style.color "black"
            ]
            prop.onClick (fun ev -> 
                let positions = Global.Types.getPositions ev

                (dispatch, positions) |>
                (
                     Types.Write_To_File >>
                     dispatch
                )
            )
        ]
    | _ -> Html.none

let textArea dispatch =
    Html.textarea[
       prop.className "textarea"
       prop.placeholder "Write down content here!"
       prop.onChange ( fun (ev : Browser.Types.Event) ->
            ev |>
            (
                Types.Msg.Change_File_Content >>
                dispatch
            )
        )
    ]

let root ( model : Model ) dispatch =
    Html.div[
        prop.children[
            Html.div[
                prop.className "columns is-centered"
                prop.children[
                    Html.div[
                        prop.className "column is-1"
                        prop.children[
                            textArea dispatch
                        ]
                    ]
                ]
            ]
            Html.div[
                prop.className "columns is-centered"
                prop.children[
                    Html.div[
                        prop.className "column is-1"
                        prop.children[
                            write2FileButton model dispatch
                        ]
                    ]
                ]
            ]
        ]
    ]

namespace Components

open Browser
open Browser.Types
open Fable.Core.JsInterop

open Oxpecker.Solid

open Types
open Data
open State

[<AutoOpen>]
module ThemeChooser =

    type Storage with
        static member tryGetItem (key: string) (storage: Storage) =
            match isIn key storage with
            | true -> storage.getItem(key) |> Some
            | false -> None

    let darkIcon =
        RawNode.html
            """<svg id="theme-toggle-dark-icon" class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z"></path></svg>"""

    let lightIcon =
        RawNode.html
            """<svg id="theme-toggle-light-icon" class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414zM4 11a1 1 0 100-2H3a1 1 0 000 2h1z" fill-rule="evenodd" clip-rule="evenodd"></path></svg>"""

    let darkTheme, setDarkTheme = createSignal false

    [<SolidComponent>]
    let ThemeChooser () =

        onMount(fun _ ->
            let isDarkPreffered = window.matchMedia("(prefers-color-scheme: dark)").matches
            let localTheme = Storage.tryGetItem "theme" localStorage

            match store.theme.IsAuto, localTheme.IsSome with
            | _, true ->
                setDarkTheme(localTheme.Value = "dark")
                setStore.Path.Map(_.theme).Update(if localTheme.Value = "dark" then Dark else Light)
            | true, _ -> setDarkTheme(isDarkPreffered)
            | _ -> ())

        createEffect(fun () ->
            if (darkTheme()) then
                document.documentElement.classList.add("dark")
                if not store.theme.IsAuto then
                    localStorage.setItem("theme", "dark")
                    setStore.Path.Map(_.theme).Update(Dark)
            else
                document.documentElement.classList.remove("dark")
                if not store.theme.IsAuto then
                    localStorage.setItem("theme", "light")
                    setStore.Path.Map(_.theme).Update(Light))
        onMount(fun () ->
            let theme = Storage.tryGetItem "theme" localStorage
            // printfn "Theme: %A" theme
            let isDarkPreffered = window.matchMedia("(prefers-color-scheme: dark)").matches
            // printfn "Is dark preffered: %b" isDarkPreffered
            (theme = Some "dark" || theme.IsNone && isDarkPreffered) |> setDarkTheme)

        button(
            type' = "button",
            class' =
                "h-5 w-5 text-gray-500 dark:text-gray-400 not-md:text-gray-200 not-md:place-self-end hover:bg-gray-100 dark:hover:bg-gray-700 focus:outline-none focus:ring-4 focus:ring-gray-200 dark:focus:ring-gray-700 rounded-lg text-sm",
            onClick = fun _ -> setDarkTheme(not(darkTheme()))
        ) {
            div(class' = "contents").classList({| ``hidden`` = not(darkTheme()) |}) { lightIcon() }
            div(class' = "contents").classList({| ``hidden`` = darkTheme() |}) { darkIcon() }
        }

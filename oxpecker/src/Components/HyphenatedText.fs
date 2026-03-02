namespace Oxpecker.Solid.Imports

open Oxpecker.Solid
open Fable.Core

[<Erase; Import("HyphenatedText", "../Components/HyphenatedText")>]
type HyphenatedText() =
    interface RegularNode

[<AutoOpen>]
module HyphenatedText =
    [<Import("hyphenateSync", "hyphen/fr")>]
    let hyphenate (text: string) : string = jsNative

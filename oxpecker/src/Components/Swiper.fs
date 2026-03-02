namespace Oxpecker.Solid.Imports

open Oxpecker.Solid
open Fable.Core

[<CompiledName("swiper-container")>]
type SwiperContainer() =
    interface RegularNode

[<CompiledName("swiper-slide")>]
type SwiperSlide() =
    interface RegularNode

module Swiper =
    [<Import("register", "swiper/element/bundle")>]
    let register () : unit = jsNative

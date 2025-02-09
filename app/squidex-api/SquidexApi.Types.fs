namespace rec SquidexApi

[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type UnitDataTagsEnum =
    /// atelier
    | [<CompiledName "atelier">] Atelier
    /// programmation
    | [<CompiledName "programmation">] Programmation
    /// boutique
    | [<CompiledName "boutique">] Boutique
    /// accueil
    | [<CompiledName "accueil">] Accueil

[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type AssetType =
    | [<CompiledName "UNKNOWN">] Unknown
    | [<CompiledName "IMAGE">] Image
    | [<CompiledName "AUDIO">] Audio
    | [<CompiledName "VIDEO">] Video

[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type EnrichedAssetEventType =
    | [<CompiledName "CREATED">] Created
    | [<CompiledName "DELETED">] Deleted
    | [<CompiledName "ANNOTATED">] Annotated
    | [<CompiledName "UPDATED">] Updated

[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type EnrichedContentEventType =
    | [<CompiledName "CREATED">] Created
    | [<CompiledName "DELETED">] Deleted
    | [<CompiledName "PUBLISHED">] Published
    | [<CompiledName "STATUS_CHANGED">] StatusChanged
    | [<CompiledName "UPDATED">] Updated
    | [<CompiledName "UNPUBLISHED">] Unpublished
    | [<CompiledName "REFERENCE_UPDATED">] ReferenceUpdated

/// The structure of the À propos data input type.
type AboutDataInputDto =
    { content: Option<AboutDataContentInputDto> }

/// The structure of the Contenu field of the À propos content input type.
type AboutDataContentInputDto =
    { en: Option<string>
      fr: Option<string> }

/// The structure of the Page data input type.
type PageDataInputDto =
    { unit: Option<PageDataUnitInputDto>
      medias: Option<PageDataMediasInputDto>
      id: Option<PageDataIdInputDto> }

/// The structure of the Titre/Description/Contenu field of the Page content input type.
type PageDataUnitInputDto =
    { en: Option<string>
      fr: Option<string> }

/// The structure of the Médias field of the Page content input type.
type PageDataMediasInputDto = { iv: Option<string> }
/// The structure of the Identifiant URL field of the Page content input type.
type PageDataIdInputDto = { iv: Option<string> }

/// The structure of the Informations pratiques data input type.
type ContactDataInputDto =
    { content: Option<ContactDataContentInputDto> }

/// The structure of the Contenu field of the Informations pratiques content input type.
type ContactDataContentInputDto =
    { en: Option<string>
      fr: Option<string> }

/// The error returned by the GraphQL backend
type ErrorType = { message: string }

[<RequireQualifiedAccess>]
module rec SquidexApi.Pages

type UnitComponent =
    { title: Option<string>
      short: Option<string>
      text: Option<string> }

type PageDataUnitDto = { fr: Option<UnitComponent> }

type Asset =
    {
        /// The URL to the asset.
        url: string
        /// The thumbnail URL to the asset.
        thumbnailUrl: Option<string>
        /// The file type (file extension) of the asset.
        fileType: string
        /// The file name of the asset.
        fileName: string
        /// The hash of the file. Can be null for old files.
        fileHash: string
    }

type CarouselComponent = { medias: Option<list<Asset>> }
type PageDataMediasDto = { iv: Option<list<CarouselComponent>> }

/// The data of the content.
type PageDataDto =
    { unit: Option<PageDataUnitDto>
      medias: Option<PageDataMediasDto> }

/// Query Page content items.
type Page =
    {
        /// The data of the content.
        data: PageDataDto
    }

/// The app queries.
type Query =
    {
        /// Query Page content items.
        queryPageContents: Option<list<Page>>
    }

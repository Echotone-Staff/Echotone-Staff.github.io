[<RequireQualifiedAccess>]
module rec SquidexApi.Aboutcontact

type UnitComponent =
    { title: Option<string>
      short: Option<string>
      text: Option<string> }

type AboutDataContentDto = { fr: Option<UnitComponent> }

/// The data of the content.
type AboutDataDto =
    { content: Option<AboutDataContentDto> }

/// Find an À propos singleton.
type About =
    {
        /// The data of the content.
        data: AboutDataDto
    }

type UnitDataLocationsChildDto = { location: Option<string> }

type Fr =
    { title: Option<string>
      short: Option<string>
      text: Option<string>
      locations: Option<list<UnitDataLocationsChildDto>> }

type ContactDataContentDto = { fr: Option<Fr> }

/// The data of the content.
type ContactDataDto =
    { content: Option<ContactDataContentDto> }

/// Find an Informations pratiques singleton.
type Contact =
    {
        /// The data of the content.
        data: ContactDataDto
    }

/// The app queries.
type Query =
    {
        /// Find an À propos singleton.
        findAboutSingleton: Option<About>
        /// Find an Informations pratiques singleton.
        findContactSingleton: Option<Contact>
    }

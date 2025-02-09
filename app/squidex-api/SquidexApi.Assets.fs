[<RequireQualifiedAccess>]
module rec SquidexApi.Assets

/// Get assets.
type Asset =
    {
        /// The ID of the object (usually GUID).
        id: string
        /// The thumbnail URL to the asset.
        thumbnailUrl: Option<string>
        /// The file name of the asset.
        fileName: string
        /// The hash of the file. Can be null for old files.
        fileHash: string
        /// The file type (file extension) of the asset.
        fileType: string
        /// The size of the file in bytes.
        fileSize: int
        /// The file name as slug.
        slug: string
        /// The asset tags.
        tags: list<string>
        /// The source URL of the asset.
        sourceUrl: Option<string>
        /// The timestamp when the object was updated the last time.
        lastModified: string
        /// The URL to the asset.
        url: string
    }

/// The app queries.
type Query =
    {
        /// Get assets.
        queryAssets: list<Asset>
    }

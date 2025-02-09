#r "fsproj: site-gen/SiteGen.fsproj"

open SiteGen.Gen
open FsHttp
open Site

refreshAssets ()
let token = Config.getToken ()

http {
    config_useBaseUrl "https://laab.ddns.net:38080"
    AuthorizationBearer token
    GET "/api/content/echotone/swagger/v1/swagger.json"
}
|> Request.send
|> Response.toText

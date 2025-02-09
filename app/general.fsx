#r "fsproj: site-utils/SiteUtils.fsproj"

open Site

Config.refresh ()

Config.getToken ()

Config.refreshSchema ()

Build notes:

When building locally:
- Add reference to ICEBG.Blazor to ICEBG.Web.UserInterface (allows load of the web assembly framework)

When pushing to github for an Azure build:
- Remove that reference (otherwise the publish will fail)
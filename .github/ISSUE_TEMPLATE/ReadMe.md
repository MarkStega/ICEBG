## Build notes:

## There is an annoying build failure on Azure and a runtime failure in development mode when building locally and deploying to WASM.
- When building locally:
  - Needs a reference to ICEBG.Blazor to ICEBG.Web.UserInterface (allows load of the web assembly framework when executing a WASM build)

- When pushing to github for an Azure build that is SERVER based:
  - The reference to ICEBG.Blazor needs to be removed (otherwise the publish will fail)

- Solution:
  - ICEBG.Web.UserInterface conditionally includes references that depend upon the selected variables

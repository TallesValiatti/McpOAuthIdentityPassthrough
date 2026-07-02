# McpOAuthIdentityPassthrough
This repository demonstrates **OAuth Identity Passthrough** in **Microsoft Foundry Agent Service**, which lets a Foundry agent call a remote **MCP Server** on behalf of the signed in user instead of using an API key or a generic application identity. Every call to the MCP Server carries the real delegated access token of the user, issued by **Microsoft Entra ID**, so each tool runs with that user's actual permissions and consented scopes.

The identity chain goes from the authenticated user, through the Foundry Agent Service, through the client App Registration that represents Foundry as an OAuth client, through Microsoft Entra ID issuing the tokens, and finally to the protected MCP Server. The write-up in `Articles/` walks through the full setup using the Azure portal: creating the server and client App Registrations, exposing the API and its scopes, publishing the MCP Server to Azure App Service, wiring up the Foundry Agent Service, consenting as a user, and exercising the flow from the Foundry chat. A follow up part will cover consuming that same flow programmatically.

## What's inside
* `Articles/`: The write-up walking through the OAuth Identity Passthrough setup end to end.
* `Cars.Mcp/`: The sample MCP Server used to demonstrate the flow, playing the role of the protected resource (`custom-mcp-cars`). It's a remote MCP Server built with **ASP.NET Core** (**.NET 10**) that exposes a cars CRUD as MCP tools (`cars_list`, `cars_get`, `cars_create`, `cars_update`, `cars_delete`), protected by Microsoft Entra ID with scope based authorization policies for `Cars.Read`, `Cars.Write`, and `Cars.Delete`. Car data lives in an in memory store seeded with sample records, keeping the focus on the identity flow instead of persistence.

## Running the sample MCP Server
See `Articles/` for the full walkthrough (App Registrations, scopes, Azure App Service, and Foundry Agent Service configuration). In short, you'll need the **.NET 10 SDK**, an **Azure App Service** to host the MCP Server, a server App Registration exposing the `Cars.Read`, `Cars.Write`, and `Cars.Delete` scopes, a client App Registration with delegated permissions to it, and the **Azure CLI** signed in for deployment.

The application reads its Entra ID settings from the `AzureAd` configuration section (`Instance`, `TenantId`, `ClientId`, `Audience`), which can be provided as environment variables prefixed with `AzureAd__`. To run it locally:

```bash
cd Cars.Mcp
dotnet run
```

The server exposes the MCP endpoint at `/mcp`, which requires a valid access token, and an anonymous health check at `/healthz`. To deploy to an existing Azure App Service, update the `RESOURCE_GROUP_NAME` and `APP_SERVICE_NAME` variables in `Cars.Mcp/deploy.sh` and run `./deploy.sh`.

## References
Model Context Protocol C# SDK: https://csharp.sdk.modelcontextprotocol.io/concepts/getting-started.html  
Protected web API configuration in ASP.NET Core: https://learn.microsoft.com/en-us/entra/identity-platform/scenario-protected-web-api-app-configuration?wt.mc_id=MVP_407589  
Policy based authorization in ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0&wt.mc_id=MVP_407589  
OAuth Identity Passthrough for MCP in Foundry Agents: https://learn.microsoft.com/en-us/azure/foundry/agents/how-to/mcp-authentication?view=foundry&wt.mc_id=MVP_407589  
Configure ASP.NET Core apps on Azure App Service: https://learn.microsoft.com/en-us/azure/app-service/configure-language-dotnetcore?wt.mc_id=MVP_407589  

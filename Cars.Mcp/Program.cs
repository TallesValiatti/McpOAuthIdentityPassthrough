using Cars.Mcp.Extensions;
using Cars.Mcp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options => options.AddCarsPolicies());

builder.Services.AddSingleton<CarStore>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .AddAuthorizationFilters()
    .WithToolsFromAssembly();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/healthz", () => Results.Ok(new
{
    status = "healthy",
    service = "cars-mcp"
}))
.AllowAnonymous();

app.MapMcp("/mcp")
   .RequireAuthorization("McpAccess");

app.Run();

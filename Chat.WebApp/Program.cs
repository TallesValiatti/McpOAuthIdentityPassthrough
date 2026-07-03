using Azure.AI.Projects;
using Azure.Identity;
using Chat.WebApp.Models;
using Chat.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<FoundryOptions>(
    builder.Configuration.GetSection("Foundry"));

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var projectEndpoint = configuration["Foundry:ProjectEndpoint"];

    if (string.IsNullOrWhiteSpace(projectEndpoint))
        throw new InvalidOperationException("Missing configuration: Foundry:ProjectEndpoint");

    return new AIProjectClient(
        endpoint: new Uri(projectEndpoint),
        tokenProvider: new DefaultAzureCredential());
});

builder.Services.AddScoped<FoundryChatService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

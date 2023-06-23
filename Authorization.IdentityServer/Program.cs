
using Authorization.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddIdentityServer()
    .AddInMemoryClients(Configuration.GetClients())
    .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
    .AddInMemoryApiResources(Configuration.GetApiResources())
    .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseIdentityServer();

app.MapDefaultControllerRoute();

app.Run();

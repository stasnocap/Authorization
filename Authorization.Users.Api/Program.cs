var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

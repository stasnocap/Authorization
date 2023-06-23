using Authorization.JWTBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddAuthentication("OAuth")
    .AddJwtBearer("OAuth", config =>
    {
        var secretBytes = Encoding.UTF8.GetBytes(Constants.SecretKey);
        var key = new SymmetricSecurityKey(secretBytes);

        config.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Query.ContainsKey("t"))
                {
                    context.Token = context.Request.Query["t"];
                }

                return Task.CompletedTask;
            }
        };

        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = Constants.Issuer,
            ValidAudience = Constants.Audience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

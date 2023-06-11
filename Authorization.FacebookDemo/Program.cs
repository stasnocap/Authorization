using System.Security.Claims;
using Authorization.FacebookDemo;
using Authorization.FacebookDemo.Data;
using Authorization.FacebookDemo.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
    config.UseInMemoryDatabase("MEMORY");
})
    .AddIdentity<ApplicationUser, ApplicationRole>(config =>
    {
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddAuthentication()
    .AddFacebook(config =>
    {
        config.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        config.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    });

builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Admin/Login/";
    config.AccessDeniedPath = "/Home/AccessDenied/";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, "Administrator");
    });

    options.AddPolicy("Manager", builder =>
    {
        builder.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "Manager")
                                      || x.User.HasClaim(ClaimTypes.Role, "Administrator"));
    });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

using (var scope = app.Services.CreateScope())
{
    DatabaseInitializer.Init(scope.ServiceProvider);
}

app.Run();

namespace Authorization.FacebookDemo
{
    public static class DatabaseInitializer
    {
        public static void Init(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser()
            {
                UserName = "User",
                //Password = "123qwe",
                FirstName = "FirstName",
                LastName = "LastName",
            };

            var result = userManager.CreateAsync(user, "123qwe").GetAwaiter().GetResult();
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors));
            }

            var claims = new List<Claim>
            {
                new("Demo", "Value"),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, "Administrator")
            };

            userManager.AddClaimsAsync(user, claims).GetAwaiter().GetResult();
        }
    }
}

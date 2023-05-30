using activities;
using activities.Areas.Identity;
using activities.Data;
using activities.Extensions;
using activities.Repository.Activities;
using activities.Repository.UserProfil;
using activities.Repository.Configs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MudBlazor.Services;
using activities.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddLocalization();

builder.Services.AddMudServices();


builder.Services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat
                     .Suffix).AddDataAnnotationsLocalization(); ;


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
            new CultureInfo("en-US"),
            new CultureInfo("fr-FR"),
            new CultureInfo("es-ES")
    };
    options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IActivitiesRepository, ActivitiesRepository>();
builder.Services.AddScoped<IAppConfigsRepository, AppConfigsRepository>();
builder.Services.AddScoped<IUserProfileReposiotry, UserProfileReposiotry>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<TokenProvider>();


builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
{
    o.DetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

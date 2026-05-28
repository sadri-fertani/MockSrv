using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MockSrv.Web;
using MockSrv.Web.DbContexts;
using MockSrv.Web.Mapper;
using MockSrv.Web.Services;
using Newtonsoft.Json;
using Radzen;
using Refit;
using Serilog;
using System.Globalization;

try
{
    // Configuration Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("Logs/web-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    Log.Information("Démarrage de l'application Web MockSrv");

    var builder = WebApplication.CreateBuilder(args);

    // Permet d'éviter les problèmes de fichier 'statique' si un environnement est autre que prod ou dev (non standard)
    builder.WebHost.UseWebRoot("wwwroot").UseStaticWebAssets();

    // Par défaut, le appsettings.json est présent, on l'ajoute de façon explicite
    builder.Configuration.AddJsonFile("appsettings.json");

    builder.Host.UseSerilog();

    builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Identity.Application";
    })
    .AddBearerToken(IdentityConstants.BearerScheme)
    .AddCookie("Identity.Application", options =>
    {
        // Cookie settings
        //
    });

builder.Services
    .AddAuthorizationBuilder();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

// Ajouter les services dans le container d'injection de d�pendance
builder.Services.AddRazorPages();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services
    .AddRadzenComponents();

builder.Services
    .AddAutoMapper(cfg => cfg.AddProfile<ApplicationProfile>());

builder.Services
    .AddRefitClient<IMockServerApi>(new RefitSettings { ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings()) })
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Api:Admin"));
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            UseDefaultCredentials = true
        };
    });

#region Localisation

builder.Services.AddControllers();
builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo>()
    {
        new("en"),
        new("fr")
    };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddSingleton<LocalisationService>();
#endregion

// app Build
var app = builder.Build();

app.MapIdentityApi<IdentityUser>();

// D�but de la configuration de l'app
#region Localisation
if (app.Environment.IsDevelopment())
{
    app.UseRequestLocalization("en-US");// fr-FR <--> en-US
}
else
{
    var options = ((IApplicationBuilder)app).ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
    if (options != null)
        app.UseRequestLocalization(options.Value);
}
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app
    .MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.UseAntiforgery();

await app.RunAsync();}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application s'est arrêtée de manière inattendue");
}
finally
{
    Log.CloseAndFlush();
}
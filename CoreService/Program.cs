using CoreService.Admin;
using CoreService.Audit;
using CoreService.Auth;
using CoreService.Auth.Services;
using CoreService.Contact;
using CoreService.Content;
using CoreService.Content.Infrastructure;
using CoreService.Infrastructure;
using CoreService.Infrastructure.Persistence;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Render / tek reverse proxy: gelen X-Forwarded-* güvenilir kabul edilir.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
var useConfiguredCors = corsOrigins is { Length: > 0 };
if (useConfiguredCors || builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("NexgensoftWeb", policy =>
        {
            if (useConfiguredCors)
            {
                policy.WithOrigins(corsOrigins!);
            }
            else
            {
                policy.WithOrigins(
                    "http://localhost:5173",
                    "https://localhost:5173",
                    "http://127.0.0.1:5173",
                    "https://127.0.0.1:5173",
                    "http://localhost:5174",
                    "https://localhost:5174");
            }

            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuthFeature(builder.Configuration);
builder.Services.AddAuditFeature(builder.Configuration);
builder.Services.AddContactFeature(builder.Configuration);
builder.Services.AddContentFeature();
builder.Services.AddAdminFeature();

var supported = new[] { "tr", "en" };
builder.Services.Configure<RequestLocalizationOptions>(o =>
{
    o.SetDefaultCulture("tr")
        .AddSupportedCultures(supported)
        .AddSupportedUICultures(supported);
    o.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

var app = builder.Build();

var applyMigrations = app.Configuration.GetValue(
    "Database:ApplyMigrationsOnStartup",
    app.Environment.IsDevelopment());
if (applyMigrations)
{
    // Windows + Supabase: DNS bazen yalnızca AAAA döner; IPv6 rotası yoksa
    // "İstenen ad geçerli olduğu halde istenen türde bir veri bulunamadı" (SocketException) oluşur.
    if (app.Configuration.GetValue("Database:PreferIpv4Dns", false))
    {
        AppContext.SetSwitch("System.Net.DisableIPv6", true);
    }

    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

await using (var adminSeedScope = app.Services.CreateAsyncScope())
{
    var adminSeeder = adminSeedScope.ServiceProvider.GetRequiredService<IAdminUserSeeder>();
    await adminSeeder.SeedAdminIfEnabledAsync();
}

if (app.Configuration.GetValue("Seed:EnableSiteContentSeed", false))
{
    await using var scope = app.Services.CreateAsyncScope();
    var siteSeeder = scope.ServiceProvider.GetRequiredService<ISiteContentSeeder>();
    await siteSeeder.SeedIfEmptyAsync();
}

app.UseForwardedHeaders();
app.UseRequestLocalization();
// Render/Docker: Kestrel yalnızca HTTP; TLS edge'de. UseHttpsRedirection HTTPS portu bulamaz (warn [3]).
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

if (useConfiguredCors || app.Environment.IsDevelopment())
{
    app.UseCors("NexgensoftWeb");
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRequestAudit();

app.MapControllers();

app.Run();

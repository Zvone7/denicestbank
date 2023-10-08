using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Portal.Api;
using Portal.Bll.Generation;
using Portal.Bll.Services;
using Portal.Core.Generation;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Dbl.Providers;

var builder = WebApplication.CreateBuilder(args);


var loggerFactory = LoggerFactory.Create(c =>
{
    c.AddConsole();
    c.AddDebug();
});

var logger = loggerFactory.CreateLogger<Program>();

#if DEBUG
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
#endif
var connectionString = builder.Configuration["ConnectionStrings:DbConnectionString"];

if (String.IsNullOrWhiteSpace(connectionString))
{
    logger.LogError("ConnectionString to Database can't be empty");
    throw new ArgumentException("Missing value for DbConnectionString");
}

logger.LogInformation("*** Application started");

builder.Services.AddSingleton<IRandomGenerator>(_ => new RandomGenerator());
builder.Services.AddSingleton<IPersonProvider>(_ => new PersonProvider(connectionString));
builder.Services.AddSingleton<ILoanProvider>(_ => new LoanProvider(connectionString));
builder.Services.AddSingleton<ITransactionProvider>(_ => new TransactionProvider(connectionString));

builder.Services.AddSingleton<IPersonService, PersonService>();
builder.Services.AddSingleton<ILoanService, LoanService>();
builder.Services.AddSingleton<ITransactionService, TransactionService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

logger.LogInformation("*** DI initialized.");

AddAzureAuthenticationAndAuthorization(logger);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();


void AddAzureAuthenticationAndAuthorization(ILogger logger)
{
    logger.LogInformation("*** Start Auth&Auth setup");
    try
    {
        logger.LogInformation("*** Attempting to load all Aad properties");

        var azureAdSection = builder.Configuration.GetSection("AzureAd");
        var domain = builder.Configuration["ad-domain"];
        var instance = builder.Configuration["azure-instance"];
        var tenantId = builder.Configuration["azure-tenant-id"];
        var appClientId = builder.Configuration["portal-appreg-id"];
        var callbackPath = builder.Configuration["portal-callback-path"];

        domain.ThrowIfNullOrWhiteSpace(nameof(domain));
        instance.ThrowIfNullOrWhiteSpace(nameof(instance));
        tenantId.ThrowIfNullOrWhiteSpace(nameof(tenantId));
        appClientId.ThrowIfNullOrWhiteSpace(nameof(appClientId));
        callbackPath.ThrowIfNullOrWhiteSpace(nameof(callbackPath));

        logger.LogInformation("*** All Aad properties loaded");
        logger.LogInformation($"***_ Domain: {domain.Substring(0, 3)}");
        logger.LogInformation($"***_ TenantId: {tenantId.Substring(0, 3)}");
        logger.LogInformation($"***_ AppClientId: {appClientId.Substring(0, 3)}");

        azureAdSection.GetSection("Instance").Value = instance;
        azureAdSection.GetSection("TenantId").Value = tenantId;
        azureAdSection.GetSection("ClientId").Value = appClientId;
        azureAdSection.GetSection("CallbackPath").Value = callbackPath;
        azureAdSection.GetSection("Domain").Value = domain;


        // for Azure AD users
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);

        // for app regs
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        logger.LogInformation("*** Aad Auth&Auth initialized.");
    }
    catch (Exception e)
    {
        var message = $"Exception on {nameof(AddAzureAuthenticationAndAuthorization)}: {e.Message}";
        logger.LogError(e, message);
        System.Diagnostics.Trace.TraceError(message);
    }
}

namespace Portal.Api
{
    public static class StringExt
    {
        public static void ThrowIfNullOrWhiteSpace(this String? propertyValue, String propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyValue))
                throw new InvalidCredentialException($"Unable to retrieve mandatory fields for field {propertyName} - cannot be empty");
        }
    }
}
using System.Security.Authentication;
using Azure.Identity;
using denicestbankportal.Logic;
using denicestbankportal.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Read the connection string from appsettings.json
#if DEBUG
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
#endif
var connectionString = builder.Configuration["ConnectionStrings:DbConnectionString"];

Console.WriteLine("*** Application started");

builder.Services.AddSingleton(provider => new PersonProvider(connectionString));
builder.Services.AddSingleton(provider => new LoanProvider(connectionString));
builder.Services.AddSingleton(provider => new TransactionProvider(connectionString));

builder.Services.AddSingleton<PersonService>();
builder.Services.AddSingleton<LoanService>();
builder.Services.AddSingleton<TransactionService>();

AddAzureAuthenticationAndAuthorization();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
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


void AddAzureAuthenticationAndAuthorization()
{
    Console.WriteLine("*** Start Auth&Auth setup");
    var keyVaultName = builder.Configuration["KeyVaultName"];
    try
    {
        if (!builder.Environment.IsDevelopment())
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions()));

        Console.WriteLine("*** Attempting to load all Aad properties");
        
        var azureAdSection = builder.Configuration.GetSection("AzureAd");
        var domain = builder.Configuration["portal-domain"];
        var instance = builder.Configuration["azure-instance"];
        var tenantId = builder.Configuration["azure-tenant-id"];
        var appClientId = builder.Configuration["portal-appreg-id"];
        var callbackPath = builder.Configuration["portal-callback-path"];

        domain.ThrowIfNullOrWhiteSpace(nameof(domain));
        instance.ThrowIfNullOrWhiteSpace(nameof(instance));
        tenantId.ThrowIfNullOrWhiteSpace(nameof(tenantId));
        appClientId.ThrowIfNullOrWhiteSpace(nameof(appClientId));
        callbackPath.ThrowIfNullOrWhiteSpace(nameof(callbackPath));

        Console.WriteLine("*** All Aad properties loaded");

        azureAdSection.GetSection("Instance").Value = instance;
        azureAdSection.GetSection("TenantId").Value = tenantId;
        azureAdSection.GetSection("ClientId").Value = appClientId;
        azureAdSection.GetSection("CallbackPath").Value = callbackPath;
        azureAdSection.GetSection("Domain").Value = domain;
        
        
        // for Azure AD users
        // builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
        
        // for app regs
        // builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
        // Configure authentication for calling APIs
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
        
        
#if DEBUG
        IdentityModelEventSource.ShowPII = true;
        IdentityModelEventSource.LogCompleteSecurityArtifact = true;
#endif
        
        Console.WriteLine("*** Aad Auth&Auth initialized.");
    }
    catch (Exception e)
    {
        var message = $"Exception setting up Azure Authentication: {e}";
        Console.WriteLine(message);
        System.Diagnostics.Trace.TraceError(message);
    }
}

public static class StringExt
{
    public static void ThrowIfNullOrWhiteSpace(this String? propertyValue, String propertyName)
    {
        if (String.IsNullOrWhiteSpace(propertyValue))
            throw new InvalidCredentialException($"Unable to retrieve mandatory fields for field {propertyName} - cannot be empty");
    }
}
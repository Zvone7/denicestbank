using System.Security.Authentication;
using Azure.Identity;
using denicestbankportal.Logic;
using denicestbankportal.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Read the connection string from appsettings.json
#if DEBUG
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
#endif
var connectionString = builder.Configuration["ConnectionStrings:DbConnectionString"];

Console.WriteLine("Application started");

// Add PersonProvider and PersonService to the container.
builder.Services.AddSingleton(provider => new PersonProvider(connectionString));
builder.Services.AddSingleton(provider => new LoanProvider(connectionString));
builder.Services.AddSingleton(provider => new TransactionProvider(connectionString));

builder.Services.AddSingleton<PersonService>();
builder.Services.AddSingleton<LoanService>();
builder.Services.AddSingleton<TransactionService>();

AddAzureAuthenticationAndAuthorization();

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

app.MapControllerRoute(
    name: "catch-all",
    pattern: "{controller=Home}/{action=Index}");

app.Run();


void AddAzureAuthenticationAndAuthorization()
{
    Console.WriteLine("Add Auth&Auth");
    var keyVaultName = builder.Configuration["KeyVaultName"];
    try
    {
        // Add Azure Key Vault configuration
        if (!builder.Environment.IsDevelopment())
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions()));

        builder.Services
            .AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);

        Console.WriteLine("Attempting to load all Aad properties");
        // Add Authorization using Managed Identity, App Registrations, and App Roles
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
        Console.WriteLine("All Aad properties loaded");

        if (String.IsNullOrWhiteSpace(instance) ||
            String.IsNullOrWhiteSpace(tenantId) ||
            String.IsNullOrWhiteSpace(appClientId))
            throw new InvalidCredentialException($"Unable to retrieve mandatory fields for Azure Auth - " +
                                                 $"Instance, TenantId, ClientId cannot be empty");

        azureAdSection.GetSection("Instance").Value = instance;
        azureAdSection.GetSection("TenantId").Value = tenantId;
        azureAdSection.GetSection("ClientId").Value = appClientId;
        azureAdSection.GetSection("CallbackPath").Value = callbackPath;
        azureAdSection.GetSection("Domain").Value = domain;
#if DEBUG
        IdentityModelEventSource.ShowPII = true;
#endif

        ///////here
        builder.Services
            .AddAuthentication(AzureADDefaults.AuthenticationScheme)
            .AddAzureAD(options => builder.Configuration.Bind("AzureAd", options));
        Console.WriteLine("Aad auth initialized.");
    }
    catch (Exception e)
    {
        var message = $"{DateTime.Now}|Exception setting up Azure Authentication: KeyVault {keyVaultName}: {e}";
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
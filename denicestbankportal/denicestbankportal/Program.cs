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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


void AddAzureAuthenticationAndAuthorization()
{
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

        // Add Authorization using Managed Identity, App Registrations, and App Roles
        var azureAdSection = builder.Configuration.GetSection("AzureAd");
        var instance = builder.Configuration["azure-instance"];
        var tenantId = builder.Configuration["azure-tenant-id"];
        var appClientId = builder.Configuration["portal-appreg-id"];
        var domain = builder.Configuration["portal-domain"];
        var callbackPath = builder.Configuration["portal-callback-path"];

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
    }
    catch (Exception e)
    {
        var message = $"{DateTime.Now}|Exception setting up Azure Authentication: KeyVault {keyVaultName}: {e}";
        Console.WriteLine(message);
        System.Diagnostics.Trace.TraceError(message);
    }
}

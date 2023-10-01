using System.Transactions;
using denicestbankportal.Logic;
using denicestbankportal.Database;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Read the connection string from appsettings.json
var configuration = builder.Configuration;
var connectionString = configuration["ConnectionString"];

// Add PersonProvider and PersonService to the container.
builder.Services.AddSingleton(provider => new PersonProvider(connectionString));
builder.Services.AddSingleton(provider => new LoanProvider(connectionString));
builder.Services.AddSingleton(provider => new TransactionProvider(connectionString));

builder.Services.AddSingleton<PersonService>();
builder.Services.AddSingleton<LoanService>();
builder.Services.AddSingleton<TransactionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
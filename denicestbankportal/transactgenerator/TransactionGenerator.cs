using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace transactgenerator
{
    public class TransactionGenerator
    {
        [FunctionName("TransactionGenerator")]
        public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log)
        {

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var appSettingsFilePath = "appsettings.json";
#if DEBUG
            appSettingsFilePath = "appsettings.local.json";
#endif

            string jsonData = File.ReadAllText(appSettingsFilePath);
            
            var adSecrets = JsonConvert.DeserializeObject<AzureAdSecrets>(jsonData);
            
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(adSecrets.TgAppId)
                .WithClientSecret(adSecrets.TgSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{adSecrets.TenantId}"))
                .Build();
            
            string[] scopes = new string[] { $"api://{adSecrets.PortalAppId}/.default" }; // Replace with your target API scope
            
            AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            
            if (result != null)
            {
                string accessToken = result.AccessToken;
            
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
                HttpResponseMessage response = await client.PostAsync($"https://{adSecrets.PortalDomain}/api/transaction/GenerateTransactions", null);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var transactions = JsonConvert.DeserializeObject<List<Transact>>(content);
                    if (!transactions.Any())
                    {
                        Console.WriteLine("No transactions performed.");
                    }
                    foreach (var transaction in transactions)
                    {
                        Console.WriteLine($"{transaction.PersonId} -> {transaction.LoanId}: {transaction.Amount}");
                    }
                }
                else
                {
                    Console.WriteLine($"Exception on request." +
                                      $"Status code {response.StatusCode}" +
                                      $"Content:{response.Content}");
                }
            }
        }
    }


    public class AzureAdSecrets
    {
        [JsonProperty("azure-instance")] public string Instance { get; set; }

        [JsonProperty("azure-tenant-id")] public string TenantId { get; set; }

        [JsonProperty("portal-appreg-id")] public string PortalAppId { get; set; }
        [JsonProperty("portal-domain")]
        public string PortalDomain { get; set; }

        [JsonProperty("transact-generator-appreg-id")]
        public string TgAppId { get; set; }

        [JsonProperty("transact-generator-secret")]
        public string TgSecret { get; set; }
    }
    public class Transact
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public Guid LoanId { get; set; }
        public DateTime UpdateDatetimeUtc { get; set; }
        public decimal Amount { get; set; }
    }
}
        
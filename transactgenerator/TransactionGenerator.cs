using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace transactgenerator
{
    public class TransactionGenerator
    {
        [FunctionName("TransactionGenerator")]
        public async Task Run([TimerTrigger(
#if DEBUG
                scheduleExpression:"* * * * * *",
#else
                scheduleExpression:"0 */45 * * * *",
#endif
                RunOnStartup = true)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("secrets.json")
#if DEBUG
                .AddJsonFile("secrets.local.json", optional: true, reloadOnChange: true)
#endif
                .AddEnvironmentVariables()
                .Build();

            var adSecrets = new AzureAdSecrets()
            {
                Instance = config["azure-instance"],
                TenantId = config["azure-tenant-id"],
                PortalAppId = config["portal-appreg-id"],
                PortalDomain = config["portal-appservice-domain"],
                TgAppId = config["transact-generator-appreg-id"],
                TgSecret = config["transact-generator-secret"],
            };

            if (String.IsNullOrEmpty(adSecrets.TgSecret))
            {
                log.LogInformation("AdSecrets are null");
            }

            log.LogInformation($"TenantId: {adSecrets.TenantId.Substring(0, 3)}");
            log.LogInformation($"TgAppId: {adSecrets.TgAppId.Substring(0, 3)}");
            log.LogInformation($"TgSecret: {adSecrets.TgSecret.Substring(0, 3)}");
            log.LogInformation($"Domain: {adSecrets.PortalDomain}");


            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(adSecrets.TgAppId)
                .WithClientSecret(adSecrets.TgSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{adSecrets.TenantId}"))
                .Build();

            string[] scopes = new string[]
                { $"api://{adSecrets.PortalAppId}/.default" }; // Replace with your target API scope

            AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            log.LogInformation($"Received token.");

            if (result != null)
            {
                string accessToken = result.AccessToken;

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                log.LogInformation($"Making request to generate transactions.");

                HttpResponseMessage response =
                    await client.PostAsync($"https://{adSecrets.PortalDomain}/api/transaction/GenerateTransactions",
                        null);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var transactions = JsonConvert.DeserializeObject<List<Transact>>(content);
                    if (!transactions.Any())
                    {
                        log.LogInformation("No transactions performed.");
                    }

                    foreach (var transaction in transactions)
                    {
                        log.LogInformation($"{transaction.PersonId} -> {transaction.LoanId}: {transaction.Amount}");
                    }
                }
                else
                {
                    log.LogInformation($"Exception on request." +
                                       $"Status code {response.StatusCode}" +
                                       $"Content:{response.Content}");
                }
            }
        }
    }


    public class AzureAdSecrets
    {
        public string Instance { get; set; }

        public string TenantId { get; set; }

        public string PortalAppId { get; set; }
        public string PortalDomain { get; set; }

        public string TgAppId { get; set; }

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

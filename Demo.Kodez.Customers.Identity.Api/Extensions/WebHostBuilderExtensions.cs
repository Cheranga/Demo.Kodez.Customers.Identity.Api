using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Demo.Kodez.Customers.Identity.Api.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static void RegisterAzureAppConfigurationProviders(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureAppConfiguration(builder =>
            {
                var config = builder.Build();
                var identityAzureAppConfigurationUri = config["IdentityAzureAppConfigurationUrl"];
                var sharedAzureAppConfigurationUri = config["SharedAzureAppConfigurationUrl"];

                var credentials = new DefaultAzureCredential();

                builder.AddAzureAppConfiguration(options =>
                    {
                        options
                            .Connect(new Uri(sharedAzureAppConfigurationUri), credentials)
                            .ConfigureKeyVault(vaultOptions => { vaultOptions.SetCredential(credentials); })
                            .ConfigureRefresh(refreshOptions => { refreshOptions.Register("RefreshAll", true).SetCacheExpiration(TimeSpan.FromSeconds(5)); })
                            .UseFeatureFlags(flagOptions => { flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(5); });
                    })
                    .AddAzureAppConfiguration(options =>
                    {
                        options
                            .Connect(new Uri(identityAzureAppConfigurationUri), credentials)
                            .ConfigureKeyVault(vaultOptions => { vaultOptions.SetCredential(credentials); })
                            .ConfigureRefresh(refreshOptions => { refreshOptions.Register("RefreshAll", true).SetCacheExpiration(TimeSpan.FromSeconds(5)); })
                            .UseFeatureFlags(flagOptions => { flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(5); });
                    });
            });
        }
    }
}
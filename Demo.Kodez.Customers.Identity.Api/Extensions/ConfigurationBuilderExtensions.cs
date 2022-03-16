using System;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Demo.Kodez.Customers.Identity.Api.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static void RegisterAzureAppConfigurationProviders(this IConfigurationBuilder webHostBuilder, HostBuilderContext context, IConfigurationRoot configuration)
        {
            var credentials = new DefaultAzureCredential();

            webHostBuilder.AddAzureAppConfiguration(options =>
                {
                    var sharedAzureAppConfigurationUri = configuration["SharedAzureAppConfigurationUrl"];

                    options
                        .Connect(new Uri(sharedAzureAppConfigurationUri), credentials)
                        .ConfigureKeyVault(vaultOptions => { vaultOptions.SetCredential(credentials); })
                        .ConfigureRefresh(refreshOptions => { refreshOptions.Register("RefreshAll", true).SetCacheExpiration(TimeSpan.FromSeconds(5)); })
                        .UseFeatureFlags(flagOptions => { flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(5); });
                })
                .AddAzureAppConfiguration(options =>
                {
                    var identityAzureAppConfigurationUri = configuration["IdentityAzureAppConfigurationUrl"];
                    options
                        .Connect(new Uri(identityAzureAppConfigurationUri), credentials)
                        .ConfigureKeyVault(vaultOptions => { vaultOptions.SetCredential(credentials); })
                        .ConfigureRefresh(refreshOptions => { refreshOptions.Register("RefreshAll", true).SetCacheExpiration(TimeSpan.FromSeconds(5)); })
                        .UseFeatureFlags(flagOptions => { flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(5); });
                });
        }
    }
}
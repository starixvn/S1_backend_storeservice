using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace StoreHangfireService.Extensions;

public static class ConfigurationManagerExtension
{
    public static ConfigurationManager AddRemoteJsonConfig(this ConfigurationManager configuration, string remoteUrl)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
            return configuration;

        using var client = new HttpClient();
        configuration.AddJsonStream(client.GetAsync(remoteUrl).Result.Content.ReadAsStreamAsync().Result);
        return configuration;
    }
}

using Microsoft.Extensions.Configuration;
using System.IO;

namespace FileServices
{
    public class AppSettingHelper
    {
        public static string GetAppsetting(string key)
        {
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");
            var config = builder.Build();
            foreach (var provider in config.Providers)
            {
                provider.TryGet(key, out var value);
                return value;
            }
            return "";
        }
    }
}

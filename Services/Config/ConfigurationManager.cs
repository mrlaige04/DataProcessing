using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace DataProcessing.Services.Config
{
    public class ConfigurationManager
    {
        private readonly IConfiguration configuration;
        public ConfigurationManager()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
        public string? GetValue(string key)
        { 
            return configuration[key];
        }
        
        public IEnumerable<string?> GetSection(string key)
        {           
            var section = configuration.GetSection(key)
                .AsEnumerable()
                .Where(x=>x.Value != null)
                .Select(x=>x.Value);
            return section;
        }
    }
}

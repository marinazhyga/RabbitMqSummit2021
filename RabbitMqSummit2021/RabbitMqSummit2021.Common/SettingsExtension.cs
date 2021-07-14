using Microsoft.Extensions.Configuration;

namespace RabbitMqSummit2021.Common
{
    public static class SettingsExtension
    {
        public static RabbitMqSettings Load()
        {

            var builder = new ConfigurationBuilder()
              .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            var username = config["RabbitMq:Username"];
            var password = config["RabbitMq:Password"];
            var url = config["RabbitMq:Url"];

            var rmqSettings = new RabbitMqSettings 
            { 
                Username = string.IsNullOrEmpty(username) ? "guest" : username,
                Password = string.IsNullOrEmpty(password) ? "guest" : password,
                Url = string.IsNullOrEmpty(url) ? "rabbitmq://localhost/" : url
            };

            return rmqSettings;
        }
    }
}

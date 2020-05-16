using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksBase.Shared
{
    public class Initializator
    {
        public static (ServiceProvider, ServiceCollection) GetContainer(string[] args = null)
        {
            var collection = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[] { })
                .Build();
            collection.AddSingleton<IConfiguration>(config);
            return (collection.BuildServiceProvider(), collection);
        }
    }
}

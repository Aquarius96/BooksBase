using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BooksBase.DataAccess
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("DbConnection");
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new DataContext(options);
        }
    }
}

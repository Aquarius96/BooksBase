using Autofac;
using Autofac.Extensions.DependencyInjection;
using BooksBase.DataAccess;
using BooksBase.Infrastructure;
using BooksBase.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Web.Permissions;

namespace BooksBase.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            var (serviceProvider, collection) = Initializator.GetContainer(args);

            var factory = new AutofacServiceProviderFactory();
            var builder = factory.CreateBuilder(collection);

            builder.Register(c =>
            {
                var config = c.Resolve<IConfiguration>();

                var opt = new DbContextOptionsBuilder<DataContext>();
                opt.UseSqlServer(config.GetConnectionString("DbConnection"));

                return opt.Options;
            }).AsSelf().SingleInstance();

            builder.RegisterType<DataContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            var finalProvider = factory.CreateServiceProvider(builder);

            using var scope = finalProvider.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                Console.WriteLine("All:");
                context.Database.GetMigrations().ToList().ForEach(m => Console.WriteLine($"\t{m}"));
                Console.WriteLine("Applied:");
                context.Database.GetAppliedMigrations().ToList().ForEach(m => Console.WriteLine($"\t{m}"));
                Console.WriteLine("Pending:");
                context.Database.GetPendingMigrations().ToList().ForEach(m => Console.WriteLine($"\t{m}"));
                context.Database.Migrate();
                Console.WriteLine("Database has been migrated.");

                var permissionService = GetPermissionService();
                (context as IContext)?.SeedDatabase(permissionService);
                Console.WriteLine("Database has been seeded.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }           
        }

        private static PermissionService GetPermissionService()
        {
            var permissionService = new PermissionService();
            permissionService.AddPermissions(typeof(CorePermissions));
            return permissionService;
        }
    }
}

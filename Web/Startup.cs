using System;
using System.Linq;
using System.Text;
using Autofac;
using AutoMapper;
using BooksBase.DataAccess;
using BooksBase.Infrastructure;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Web.Permissions;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IPermissionService>(provider =>
            {
                var permissionService = new PermissionService();
                permissionService.AddPermissions(typeof(CorePermissions));

                return permissionService;
            });

            var connectionString = Configuration.GetConnectionString("DbConnection");
            services.AddDbContext<DataContext>(o => o.UseSqlServer(connectionString));
            services.AddIdentity<User, Role>(opt => opt.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
            services.AddMediatR(typeof(Startup).Assembly);
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddControllers(opt =>
                    opt.Filters.Add(typeof(ValidateActionFilter)))
                .AddNewtonsoftJson()
                .AddFluentValidation(opt =>
                {
                    opt.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Books base", Version = "v1" });
            });

            services.Configure<AuthSettings>(options => Configuration.GetSection("Authentication").Bind(options));
            services.AddOptions();
            var authentication = Configuration.GetSection("Authentication");
            var authSettings = authentication.Get<AuthSettings>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidAudience = authSettings.Audience,
                        ValidIssuer = authSettings.Issuer,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret))
                    };
                });
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IConfigureOptions<AuthorizationOptions>, ConfigureAuthorizationOptions>();
            services.AddAuthorization();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books base V1");
            });            
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            .Where(t => typeof(IService).IsAssignableFrom(t))
            .AsImplementedInterfaces();

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
        }
    }

    public class ConfigureAuthorizationOptions : IConfigureOptions<AuthorizationOptions>
    {
        private readonly IPermissionService _permissionService;
        public ConfigureAuthorizationOptions(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public void Configure(AuthorizationOptions options)
        {            
            foreach (var permission in _permissionService.GetAllPermissions())
            {
                options.AddPolicy(permission.Claim, builder => builder.AddRequirements(new PermissionRequirement(permission.Claim)));
            }
        }
    }
}

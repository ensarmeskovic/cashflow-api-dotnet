using System;
using System.Text;
using Cashflow.Common.Services.Cryptography;
using Cashflow.Common.Services.Email;
using Cashflow.Common.Services.RandomGenerator;
using Cashflow.Common.Services.TokenProcessor;
using Cashflow.Core.Configurations;
using Cashflow.Infrastructure;
using Cashflow.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Cashflow.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppSettingsConfiguration _appSettingsConfiguration = new AppSettingsConfiguration();

        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

            BuildConfiguration().Bind("Configuration", _appSettingsConfiguration);
        }

        private IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(_webHostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{_webHostEnvironment.EnvironmentName.ToLowerInvariant()}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"{_appSettingsConfiguration.ApplicationName} API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Ensar Mešković",
                        Email = "meskovic.ensar@gmail.com",
                        Url = new Uri("https://ema.health")
                    }
                });
            });
            services.AddRouting(options => options.LowercaseUrls = true);

            ConfigureAuthorization(services);
            ConfigureDatabase(services);
            ConfigureUnitOfWork(services);
            ConfigureCustomServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{_appSettingsConfiguration.ApplicationName} API V1");
                c.RoutePrefix = string.Empty;
            });
        }


        #region Custom services
        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingsConfiguration.Token.SecurityString)),

                            ValidateIssuer = true,
                            ValidIssuer = _appSettingsConfiguration.Token.Issuer,
                            ValidateAudience = true,
                            ValidAudience = _appSettingsConfiguration.Token.Audience,

                            RequireExpirationTime = true, //false
                            ValidateLifetime = true
                        };
                    });
        }
        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<CashflowContext>(options => options.UseSqlServer(_appSettingsConfiguration.Database.ConnectionString));
        }

        private static void ConfigureUnitOfWork(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private void ConfigureCustomServices(IServiceCollection services)
        {
            #region Core

            services.AddSingleton(_appSettingsConfiguration);
            services.AddSingleton(_appSettingsConfiguration.Database);
            services.AddSingleton(_appSettingsConfiguration.Email);
            services.AddSingleton(_appSettingsConfiguration.Token);
            services.AddSingleton(_appSettingsConfiguration.Firebase);

            #endregion

            #region Common

            //services.AddSingleton<IPushNotification, PushNotification>();

            services.AddScoped<ITokenProcessor, TokenProcessor>();
            services.AddScoped<ICryptography, Cryptography>();
            services.AddScoped<IRandomGenerator, RandomGenerator>();
            services.AddScoped<IEmailer, Emailer>();

            #endregion
        }
        #endregion
    }
}

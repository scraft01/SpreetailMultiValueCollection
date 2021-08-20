using SpreetailMultiValueDictionary.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SpreetailMultiValueDictionary.Services;

namespace SpreetailMultiValueDictionary
{
    public class Program
    {
        /// <summary>
        ///     Configuration for the main program
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Returns async task instead of void to support async programming.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void Main(string[] args)
        {

            try
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appSettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                        optional: true, reloadOnChange: true)
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();

                // Personalize console
                Console.Title = "Spreetail MultiValueDictionary";


                // Configure logger and log initialization of application
                Log.Logger = ConfigureLogger();
                Log.Information("Application starting");

                // Create service collection and configure our services
                var services = ConfigureServices();
                // Generate a provider
                var serviceProvider = services.BuildServiceProvider();

                // Kick off the actual application
                serviceProvider.GetService<Application>()?.Run();

                Log.Information("Application shutting down");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        ///     Configure the services using ASP.NET CORE built-in Dependency Injection.
        /// </summary>
        /// <returns></returns>
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            // register the services
            services.AddTransient<IMultiValueDictionaryService, MultiValueDictionaryService>();
            // Configuration should be singleton as the entire application should use one
            services.AddSingleton(Configuration);
            // for strongly typed options to be injected as IOption<T> in constructors
            services.AddOptions();
            // Configure MultiValueDictionarySettings so IOption<MultiValueDictionarySettings> can be injected 
            services.Configure<MultiValueDictionarySettings>(Configuration.GetSection("MultiValueDictionarySettings"));;

            // Register the actual application entry point
            services.AddTransient<Application>();

            return services;
        }

        /// <summary>
        ///     Configure the logger
        /// </summary>
        /// <returns></returns>
        private static ILogger ConfigureLogger()
        {
            var logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .CreateLogger();

            return logger;

        }
    }
}

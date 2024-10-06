
using Serilog;

namespace dotnet_base_template
{
    public class Program
    {
        private static readonly string CurrentEnvironment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        public static Task<int> Main(string[] args)
        {
            try
            {
                ConfigureLogging();
                Log.Information($"Starting service worker. Environment:{CurrentEnvironment}");
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();

                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                Log.Fatal(ex, "Service worker terminated unexpectedly");
                return Task.FromResult(1);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfigurationBuilder SetupCunfiguration(IConfigurationBuilder configBuilder)
        {
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configBuilder.AddJsonFile(
                $"appsettings.{CurrentEnvironment}.json",
                optional: true);
            configBuilder.AddEnvironmentVariables();
            return configBuilder;
        }

        private static void ConfigureLogging()
        {
            var configuration = SetupCunfiguration(new ConfigurationBuilder())
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}

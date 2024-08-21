using SPaaSChallenge.Controllers;
using SPaaSChallenge.Services;
using System.Text.Json.Serialization;
using SPaaSChallenge.Controllers.Helpers;
using SPaaSChallenge.Models;

namespace SPaaSChallenge;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true);

        builder.Build();

        services
            .AddControllers()
            .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddLogging(logging => logging.AddConsole());

        AddDependencies(services);
    }

    private void AddDependencies(IServiceCollection services)
    {
        services.AddScoped<IProductionPlantService, ProductionPlantService>();
        services.AddScoped<IDistributionBuilder, DistributionBuilder>();
        services.AddScoped<IPowerPlantFactory, PowerPlantFactory>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
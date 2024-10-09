using System.Text.Json.Serialization;
using SPaaSChallenge.Controllers;
using SPaaSChallenge.Services;
using SPaaSChallenge.Services.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddScoped<IProductionPlantService, ProductionPlantService>();   
builder.Services.AddScoped<IPowerPlantFactory, PowerPlantFactory>();             

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>(); 
app.UseAuthorization();
app.MapControllers();

app.Run();
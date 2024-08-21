using SPaaSChallenge.Controllers;
using SPaaSChallenge.Controllers.Helpers;
using SPaaSChallenge.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductionPlantService, ProductionPlantService>();   
builder.Services.AddScoped<IDistributionBuilder, DistributionBuilder>();         
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
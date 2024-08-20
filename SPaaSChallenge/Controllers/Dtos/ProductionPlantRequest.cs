using System.Text.Json.Serialization;

namespace SPaaSChallenge.Controllers.Dtos;

public class ProductionPlantRequest
{
    
    [JsonPropertyName("load")]
    public int Load { get; set; }
    
    [JsonPropertyName("fuels")] 
    public FuelDto FuelDto { get; set; }

    [JsonPropertyName("powerplants")] 
    public PowerplantDto[] PowerPlants { get; set; }
}
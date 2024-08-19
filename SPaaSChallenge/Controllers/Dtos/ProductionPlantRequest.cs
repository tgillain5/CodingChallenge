using System.Text.Json.Serialization;

namespace SPaaSChallenge.Controllers.Dtos;

public class ProductionPlantRequest
{
    public int load { get; set; }

    
    [JsonPropertyName("fuels")] 
    public FuelsDto FuelsDto { get; set; }

    public PowerplantDto[] powerplants { get; set; }
}
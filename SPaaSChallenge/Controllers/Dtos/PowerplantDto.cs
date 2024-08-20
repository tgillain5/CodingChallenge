using System.Text.Json.Serialization;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Controllers.Dtos;

public class PowerplantDto
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("efficiency")] 
    public double Efficiency { get; set; }

    [JsonPropertyName("pmin")] 
    public double MinimumProduction { get; set; }

    [JsonPropertyName("pmax")] 
    public double MaximumProduction { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("type")] 
    public PowerPlantType Type { get; set; }
    
}
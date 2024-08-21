using System.Text.Json.Serialization;

namespace SPaaSChallenge.Controllers.Dtos;

public class DistributionDto
{
    [JsonPropertyName("name")] 
    public string PowerPlantName { get; set; }
    
    [JsonPropertyName("p")]
    public double Production { get; set; }
    
    
}
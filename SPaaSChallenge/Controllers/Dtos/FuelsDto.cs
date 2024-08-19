using System.Text.Json.Serialization;

namespace SPaaSChallenge.Controllers.Dtos;

public class FuelsDto
{
    [JsonPropertyName("gas(euro/MWh)")] public double GaseuroMWh { get; set; }

    [JsonPropertyName("kerosine(euro/MWh)")]
    public double KerosineeuroMWh { get; set; }

    [JsonPropertyName("co2(euro/ton)")] 
    public double Co2euroton { get; set; }

    [JsonPropertyName("wind(%)")] public double Wind { get; set; }
}
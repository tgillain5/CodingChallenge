using System.Text.Json.Serialization;

namespace SPaaSChallenge.Controllers.Dtos;

public class FuelDto
{
    [JsonPropertyName("gas(euro/MWh)")] 
    public double GasPriceInEuroPerMwh { get; set; }

    [JsonPropertyName("kerosine(euro/MWh)")]
    public double KerosinePriceInEuroPerMwh { get; set; }

    [JsonPropertyName("co2(euro/ton)")] 
    public double Co2PriceInEuroPerTon { get; set; }

    [JsonPropertyName("wind(%)")] 
    public double Wind { get; set; }
}
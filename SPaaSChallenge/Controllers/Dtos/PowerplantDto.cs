using System.Text.Json.Serialization;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Controllers.Dtos;

public class PowerplantDto
{
    [JsonPropertyName("name")] 
    public string Name { get; set; }

    [JsonPropertyName("type")] 
    //todo find attribut to do the asString
    public string TypeAsString { get; set; }
    
    [JsonPropertyName("efficiency")] 
    public double Efficiency { get; set; }

    [JsonPropertyName("pmin")] 
    public double MinimumProduction { get; set; }

    [JsonPropertyName("pmax")] 
    public double MaximumProduction { get; set; }


    private static PowerPlantType _defaultPowerPlantType = PowerPlantType.Unknown;

    [JsonIgnore]
    public PowerPlantType Type
    {
        get => Enum.TryParse(value: TypeAsString, ignoreCase: true, out PowerPlantType powerPlantType)
            ? powerPlantType
            : PowerPlantType.Unknown;
        set => TypeAsString = Enum.GetName(value.GetType(), value) ?? _defaultPowerPlantType.ToString();
    }



}
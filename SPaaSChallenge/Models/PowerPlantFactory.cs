using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Services;


namespace SPaaSChallenge.Models;

public static class PowerPlantFactory
{
    public static IPowerPlant Create(PowerplantDto powerPlant, FuelsDto fuelsDto)
    {
        return powerPlant.Type switch
        {
            PowerPlantType.Gasfired => new GasfiredPowerPlant(
                name: powerPlant.Name,
                efficiency: powerPlant.Efficiency,
                minimumProduction: powerPlant.MinimumProduction,
                maximumProduction: powerPlant.MaximumProduction,
                gasInEuroMWh: fuelsDto.GaseuroMWh),

            PowerPlantType.Turbojet => new TurboJetPowerPlant(
                name: powerPlant.Name,
                efficiency: powerPlant.Efficiency,
                minimumProduction: powerPlant.MinimumProduction,
                maximumProduction: powerPlant.MaximumProduction,
                kerosineInEuroMWh: fuelsDto.KerosineeuroMWh),

            PowerPlantType.Windturbine => new WindTurbinePowerPlant(
                name: powerPlant.Name,
                efficiency: powerPlant.Efficiency,
                production: powerPlant.MaximumProduction,
                windPercentage: fuelsDto.Wind),

            _ => throw new InvalidPowerPlantException("invalid powerPlant type")
        };
    }
}
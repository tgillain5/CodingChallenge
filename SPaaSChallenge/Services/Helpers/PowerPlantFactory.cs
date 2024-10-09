using System.ComponentModel;
using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services.Helpers;

public class PowerPlantFactory : IPowerPlantFactory
{
    public IPowerPlant Create(PowerplantDto powerPlant, FuelDto fuelDto)
    {
        return powerPlant.Type switch
        {
            PowerPlantType.GasFired => new GasFiredPowerPlant(
                name: powerPlant.Name,
                efficiency: powerPlant.Efficiency,
                minimumProduction: powerPlant.MinimumProduction,
                maximumProduction: powerPlant.MaximumProduction,
                gasInEuroMWh: fuelDto.GasPriceInEuroPerMwh),

            PowerPlantType.Turbojet => new TurboJetPowerPlant(
                name: powerPlant.Name,
                efficiency: powerPlant.Efficiency,
                minimumProduction: powerPlant.MinimumProduction,
                maximumProduction: powerPlant.MaximumProduction,
                kerosineInEuroMWh: fuelDto.KerosinePriceInEuroPerMwh),

            PowerPlantType.WindTurbine => new WindTurbinePowerPlant(
                name: powerPlant.Name,
                production: powerPlant.MaximumProduction,
                windPercentage: fuelDto.Wind),

            _ => throw new InvalidEnumArgumentException("invalid powerPlant type")
        };
    }
}
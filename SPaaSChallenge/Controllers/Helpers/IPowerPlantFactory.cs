using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Controllers.Helpers;

public interface IPowerPlantFactory
{
    IPowerPlant Create(PowerplantDto powerPlant, FuelDto fuelDto);
}
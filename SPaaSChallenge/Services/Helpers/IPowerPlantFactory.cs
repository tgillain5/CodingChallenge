using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Services.Helpers;

public interface IPowerPlantFactory
{
    IPowerPlant Create(PowerplantDto powerPlant, FuelDto fuelDto);
}
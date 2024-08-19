using SPaaSChallenge.Controllers.Dtos;
using SPaaSChallenge.Models;

namespace SPaaSChallenge.Controllers.Mappers;

public static class DistributionMapper
{
    public static DistributionDto Map(this Distribution source)
    {
        return new DistributionDto
        {
            PowerPlantName = source.PowerPlantName,
            Production = Math.Round(source.Production,1)
        };
    }
}
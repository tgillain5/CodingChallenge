using System.Net;
using Newtonsoft.Json;
using SPaaSChallenge.Services;

namespace SPaaSChallenge.Controllers;

public class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
{
    public (HttpStatusCode code, string message) GetResponse(Exception exception)
    {
        HttpStatusCode code;
        logger.LogWarning(exception.Message);
        switch (exception)
        {
            case DistributionImpossibleException
                or InvalidPowerPlantException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }

        return (code, JsonConvert.SerializeObject(exception.Message));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            // get the response code and message
            var (status, message) = GetResponse(exception);
            response.StatusCode = (int)status;
            await response.WriteAsync(message);
        }
    }
}
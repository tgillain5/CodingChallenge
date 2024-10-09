using System.Net;
using Newtonsoft.Json;
using SPaaSChallenge.Services;

namespace SPaaSChallenge.Controllers;

public class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
{
    public (HttpStatusCode code, string message) GetResponse(Exception exception)
    {
        HttpStatusCode code;
        
        switch (exception)
        {
            case DistributionImpossibleException:
                logger.LogWarning(exception.Message);
                code = HttpStatusCode.BadRequest;
                break;
            default:
                logger.LogError(exception.Message);
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
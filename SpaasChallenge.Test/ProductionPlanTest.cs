using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using SPaaSChallenge.Controllers.Dtos;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace SpaaSChallenge.Test;

public class Tests
{
    private readonly HttpClient _client = TestServer.Create();
    
    [Fact]
    public async Task WhenAPowerPlantWillBlockPossibleDistribution_itIsIgnored()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/IgnorePowerPlantWhenTheyWillBlockPossibleDistribution.json");
        var jsonIndentedResponse = await GetJsonIndentedResponseFromHttpResponse(httpResponse);

        //Assert
        var expectedResponseAsJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Responses/IgnorePowerPlantWhenTheyWillBlockPossibleDistributionResponse.json");
        Assert.Equal(expectedResponseAsJson, jsonIndentedResponse);
    }
    
    [Fact]
    public async Task WhenProvidedWithSampleRequest_ReturnsSampleResponse()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/ProvidedRequest.json");
        var jsonIndentedResponse = await GetJsonIndentedResponseFromHttpResponse(httpResponse);

        //Assert
        var expectedResponseAsJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Responses/ProvidedResponse.json");
        Assert.Equal(expectedResponseAsJson, jsonIndentedResponse);
    }
    
    [Fact]
    public async Task PowerPlantAreOrderByCostEfficency()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/CostEfficiency.json");
        var jsonIndentedResponse = await GetJsonIndentedResponseFromHttpResponse(httpResponse);

        //Assert
        var expectedResponseAsJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Responses/CostEfficiencyResponse.json");
        Assert.Equal(expectedResponseAsJson, jsonIndentedResponse);
    }
    
    [Fact]
    public async Task WhenIsToMuch_WindIsIgnord()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/WindIsTooMuch.json");
        var jsonIndentedResponse = await GetJsonIndentedResponseFromHttpResponse(httpResponse);

        //Assert
        var expectedResponseAsJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Responses/WindIsTooMuchResponse.json");
        Assert.Equal(expectedResponseAsJson, jsonIndentedResponse);
    }
    
    [Fact]
    public async Task WhenFirstPowerPlantMinimalProductionIsToHigh_ProductionIsDistributedOnFollowingPowerPlants()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/FirstPowerPlantMinimalProductionIsToHigh.json");
        var jsonIndentedResponse = await GetJsonIndentedResponseFromHttpResponse(httpResponse);

        //Assert
        var expectedResponseAsJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/Responses/FirstPowerPlantMinimalProductionIsToHighResponse.json");
        Assert.Equal(expectedResponseAsJson, jsonIndentedResponse);
    }
    
    [Fact]
    public async Task WhenDistributionIsImpossible_BadRequest()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/DistributionImpossible.json");

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.Contains("Impossible to provide the expected load" , await httpResponse.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task WhenAllPowerPlantMaximalProductionIsNotEnough_BadRequest()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/ProductionTooLow.json");

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.Contains("Impossible to provide the expected load" , await httpResponse.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task WhenAllPowerPlantMinimalProductionIsToHigh_BadRequest()
    {
        //Act
        var httpResponse = await GetHttpResponseForTestFile("/Requests/LoadTooLow.json");

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.Contains("Impossible to provide the expected load" , await httpResponse.Content.ReadAsStringAsync());
    }
    

    private static async Task<string> GetJsonIndentedResponseFromHttpResponse(HttpResponseMessage httpResponse)
    {
        return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(await httpResponse.Content.ReadAsStringAsync()), Formatting.Indented);
    }

    private async Task<HttpResponseMessage> GetHttpResponseForTestFile(string file)
    {
        var requestAsJson = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + file);
        var request = JsonSerializer.Deserialize<ProductionPlantRequest>(requestAsJson);
        return await _client.PostAsJsonAsync("ProductionPlan/productionplan", request);
    }
}
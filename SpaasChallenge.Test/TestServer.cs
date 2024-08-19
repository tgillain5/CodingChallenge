using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using SPaaSChallenge;

namespace SpaasChallenge.Test;

public static class TestServer
{
    public static HttpClient Create()
    {
        return new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => 
                {
                    //overwrite external dependencies here
                });
            })
            .CreateClient();
    }
}
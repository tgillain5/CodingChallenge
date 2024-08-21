using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace SpaaSChallenge.Test;

public static class TestServer
{
    public static HttpClient Create()
    {
        return new WebApplicationFactory<Program>()
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
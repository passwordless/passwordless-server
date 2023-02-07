﻿using System.Net.Http.Headers;
using System.Net;

namespace HostedFidoApi.IntegrationTests;

[Collection("AccountTestes")]
public class AccountTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private TestWebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public AccountTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async void CreateAccountWithInvalidName()
    {
        var item = """
                        {
                "AccountName": "",
                "AdminEmail": "anders@passwordless.dev" 
            }
            """;
        var header = new MediaTypeHeaderValue("application/json");
        
        
        var res = await _client.PostAsync("/account/create", new StringContent(item, header ));

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }
}
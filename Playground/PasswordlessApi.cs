using Microsoft.Extensions.Options;

public class PasswordlessApi
{
    private readonly IHttpClientFactory factory;
    private readonly IOptions<PasswordlessOptions> options;

    public PasswordlessApi(IHttpClientFactory factory, IOptions<PasswordlessOptions> options)
    {
        this.factory = factory;
        this.options = options;
    }

    public async Task<string> CreateRegisterToken(RegisterOptions registerOptions)
    {
        using(var client = SecretClient){
            var res = await client.PostAsJsonAsync("register/token", registerOptions);            
            return (await res.Content.ReadAsStringAsync()) ?? "";
        }
    }
    public HttpClient SecretClient
    {
        get
        {
            var client =  factory.CreateClient();
            client.BaseAddress = new Uri(options.Value.ApiUrl);
            client.DefaultRequestHeaders.Add("ApiSecret", options.Value.ApiSecret);
            return client;
        }
    }

    public class RegisterOptions {
        public string UserId { get; set; }
    }
}
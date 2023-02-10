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
        using (var client = SecretClient)
        {
            var res = await client.PostAsJsonAsync("register/token", registerOptions);
            return (await res.Content.ReadAsStringAsync()) ?? "";
        }
    }

    internal async Task<VerifiedUser> VerifyToken(string verifyToken)
    {
        using (var client = SecretClient)
        {
            var req = await client.PostAsJsonAsync("signin/verify", new { token = verifyToken });

            // todo: replace with better error handling
            req.EnsureSuccessStatusCode();

            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadFromJsonAsync<VerifiedUser>();
                return res;
            }

            return null;
        }
    }

    public async Task<List<PasswordlessUserSummary>> ListUsers()
    {
        using (var client = SecretClient)
        {
            var req = await client.GetAsync("users/list");

            // todo: replace with better error handling
            req.EnsureSuccessStatusCode();

            if (req.IsSuccessStatusCode)
            {
                var res = await req.Content.ReadFromJsonAsync<List<PasswordlessUserSummary>>();
                return res;
            }

            return null;
        }
    }

    public HttpClient SecretClient
    {
        get
        {
            var client = factory.CreateClient();
            client.BaseAddress = new Uri(options.Value.ApiUrl);
            client.DefaultRequestHeaders.Add("ApiSecret", options.Value.ApiSecret);
            return client;
        }
    }

    public class RegisterOptions
    {
        public string UserId { get; set; }
    }

    public class PasswordlessUserSummary {
        public string UserId { get; set; }
    }

    public class VerifiedUser
    {
        public string UserId { get; set; }
        public bool Success { get; set; }
        public string Username { get; set; }
        public DateTime Timestamp { get; set; }
        public string RpId { get; set; }
        public string Origin { get; set; }
        public string Device { get; set; }
        public string Country { get; set; }
        public string Nickname { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}


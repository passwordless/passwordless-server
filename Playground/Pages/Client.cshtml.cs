using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playground.Pages;

public class ClientModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PasswordlessApi api;

    public string RegisterToken { get; set; }

    public ClientModel(ILogger<IndexModel> logger, PasswordlessApi api)
    {
        _logger = logger;
        this.api = api;
    }

    public async Task OnGet()
    {
        RegisterToken = await api.CreateRegisterToken(new PasswordlessApi.RegisterOptions() { UserId = "123"});        
    }
}


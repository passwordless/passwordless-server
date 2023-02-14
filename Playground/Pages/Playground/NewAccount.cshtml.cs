using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playground.Pages.Playground;

public class NewAccountModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PasswordlessApi api;

    public NewAccountModel(ILogger<IndexModel> logger, PasswordlessApi api)
    {
        _logger = logger;
        this.api = api;
    }

    public void OnGet() {

    }

    public async Task<IActionResult> OnPostToken(string name, string email)
    {
        // Create new account
        var userId = Guid.NewGuid().ToString();
        var token = await api.CreateRegisterToken(new PasswordlessApi.RegisterOptions() {
            UserId = userId,
            Username = email,
            DisplayName = name
        });

        return new JsonResult(token);
    }

    public async Task<IActionResult> OnPost(string token)
    {
        var res = await api.VerifyToken(token);
        return new JsonResult(res);
    }
}


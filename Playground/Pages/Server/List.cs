using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playground.Pages;

public class ListModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PasswordlessApi api;

    public List<PasswordlessApi.PasswordlessUserSummary> Users { get; set; }

    public ListModel(ILogger<IndexModel> logger, PasswordlessApi api)
    {
        _logger = logger;
        this.api = api;
    }

    public async Task OnGet()
    {
        Users = await api.ListUsers();
    }

    public async Task<IActionResult> OnPost(string token)
    {
        var res = await api.VerifyToken(token);
        return new JsonResult(res);
    }
}


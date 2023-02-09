using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playground.Pages;

public class ClientModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public ClientModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}


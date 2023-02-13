using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playground.Pages;

public class UserModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PasswordlessApi api;

    public List<PasswordlessApi.Credential> Credentials { get; set; }

    public List<PasswordlessApi.AuditLog> AuditLogs { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; }

    public UserModel(ILogger<IndexModel> logger, PasswordlessApi api)
    {
        _logger = logger;
        this.api = api;
    }

    public async Task OnGet()
    {
        Credentials = await api.ListCredentials("123");
        AuditLogs = new List<PasswordlessApi.AuditLog> {
            new () {Timestamp = DateTime.Now, Level = "info", Message = "Signed in using credential ahjeas-aiwu12-an27s-jnb4-287hn58" },
            new () {Timestamp = DateTime.Now, Level = "danger", Message = "Authentication failed because of invalid signature. CredentialId: ahjeas-aiwu12-an27s-jnb4-287hn58." },
            new () {Timestamp = DateTime.Now, Level = "info", Message = "Signed in using credential ahjeas-aiwu12-an27s-jnb4-287hn58" },
            new () {Timestamp = DateTime.Now, Level = "info", Message = "Signed in using credential ahjeas-aiwu12-an27s-jnb4-287hn58" },
            new () {Timestamp = DateTime.Now, Level = "info", Message = "Registered new credential ahjeas-aiwu12-an27s-jnb4-287hn58" }
        };
    }

    public async Task<IActionResult> OnPost(string token)
    {
        var res = await api.VerifyToken(token);
        return new JsonResult(res);
    }
}


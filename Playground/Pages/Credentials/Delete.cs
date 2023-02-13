using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Playground.Pages;

public class CredentialDeleteModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PasswordlessApi api;
    
    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string CredentialId { get; set; }

    public PasswordlessApi.Credential Credential { get; set; }

    public CredentialDeleteModel(ILogger<IndexModel> logger, PasswordlessApi api)
    {
        _logger = logger;
        this.api = api;
    }

    public async Task<IActionResult> OnGet()
    {
        var targetCredential = PasswordlessApi.Base64Url.Decode(CredentialId);

        var credentials = await api.ListCredentials("123");

        Credential = credentials.FirstOrDefault(c => c.Descriptor.Id.SequenceEqual(targetCredential));
        // TODO: Null check and handle error
        // if(CredentialId == null) {
        //     return RedirectToPage("server/user", null, new { UserId = UserId });
        // }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await api.DeleteCredential(CredentialId);

        return RedirectToPage("/credentials/user", null, new { UserId = UserId });
    }
}


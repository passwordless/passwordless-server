using ApiHelpers;
using Service;
using Service.Helpers;
using Service.Storage;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app)
    {    
        app.MapMethods("/users/list", new[] { "get" }, async (HttpContext ctx, HttpRequest req, IStorage storage,  AccountService accountService, UserCredentialsService userService) =>
        {
            try
            {
                app.Logger.LogInformation("/users/list called");

                //userId = req.Query["userId"];                
                // todo: Add Include credentials
                // todo: Add Include Aliases
                var accountname = await accountService.ValidateSecretApiKey(req.GetApiSecret());

                var result = await userService.GetAllUsers();

                app.Logger.LogInformation("event=users/list account={0}", accountname);


                return Results.Json(result, Json.Options);
            }
            catch (ApiException e)
            {
                return ErrorHelper.FromException(e);
            }
        }).RequireCors("default");
    }
}
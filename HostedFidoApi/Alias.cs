using ApiHelpers;
using Service;
using Service.Helpers;
using Service.Models;
using Service.Storage;

public static class AliasEndpoints
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        app.MapPost("/alias", async (HttpContext ctx, HttpRequest req, IStorage storage, AccountService accountService) =>
        {
            try
            {
                app.Logger.LogInformation("/alias called");
                var payload = await req.ReadFromJsonAsync<AliasPayload>(Json.Options);

                // if payload is empty, throw exception
                if (payload == null)
                {
                    throw new ApiException("Payload is empty", 400);
                }

                var accountname = await accountService.ValidateSecretApiKey(req.GetApiSecret());
                var service = await Fido2ServiceEndpoints.Create(accountname, app.Logger, app.Configuration, storage);
                await service.SetAlias(payload);

                app.Logger.LogInformation("event=alias account={0}", accountname);

                
                return Results.Ok();
            }
            catch (ApiException e)
            {
                return ErrorHelper.FromException(e);
            }
        }).RequireCors("default");

        app.MapGet("/alias/list", async (string userId, HttpContext ctx, HttpRequest req, IStorage storage, AccountService accountService) =>
        {
            try
            {
                //app.Logger.LogInformation("/alias called");

                // if payload is empty, throw exception
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ApiException("UserId is empty", 400);
                }

                var accountname = await accountService.ValidateSecretApiKey(req.GetApiSecret());
                var service = await Fido2ServiceEndpoints.Create(accountname, app.Logger, app.Configuration, storage);
                var aliases = await service.GetAliases(userId);

                app.Logger.LogInformation("event=alias/list account={0}", accountname);


                return Results.Json(aliases);
            }
            catch (ApiException e)
            {
                return ErrorHelper.FromException(e);
            }
        }).RequireCors("default");
    }
}

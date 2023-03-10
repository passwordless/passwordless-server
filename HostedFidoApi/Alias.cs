using ApiHelpers;
using Service;
using Service.Helpers;
using Service.Models;

public static class AliasEndpoints
{
    public static void MapAliasEndpoints(this WebApplication app)
    {
        app.MapPost("/alias", async (HttpContext ctx, HttpRequest req) =>
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

                var accountname = await new AccountService(app.Logger, app.Configuration).ValidateSecretApiKey(req.GetApiSecret());
                var service = await Fido2ServiceEndpoints.Create(accountname, app.Logger, app.Configuration);
                await service.SetAlias(payload.UserId, payload.Aliases);

                app.Logger.LogInformation("event=alias account={0}", accountname);

                
                return Results.Ok();
            }
            catch (ApiException e)
            {
                return ErrorHelper.FromException(e);
            }
        }).RequireCors("default");
    }
}

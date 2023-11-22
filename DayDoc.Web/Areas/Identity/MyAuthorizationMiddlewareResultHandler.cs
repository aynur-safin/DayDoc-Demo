using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using FastEndpoints;

namespace DayDoc.Web.Areas.Identity
{
    /*
     * https://stackoverflow.com/questions/69185627/asp-net-core-return-json-response-on-unauthorized-in-a-filter-at-the-controlle
     * https://stackoverflow.com/a/69197904
     * --
     * https://learn.microsoft.com/ru-ru/aspnet/core/security/authorization/customizingauthorizationmiddlewareresponse?view=aspnetcore-7.0
     * https://benfoster.io/blog/customize-authorization-response-aspnet-core/
     */     


    public class JsonAuthorizationAttribute : Attribute
    {
        public string Message { get; set; } = "";
    }

    public class MyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            // if the authorization was forbidden and the resource had specific attribute, respond as json
            // или роут начинается с '/api'
            if (authorizeResult.Forbidden
                || authorizeResult.Challenged /*authorizeResult.Succeeded == false*/)
            {
                var endpoint = context.GetEndpoint();
                var jsonHeader = endpoint?.Metadata.GetMetadata<JsonAuthorizationAttribute>();
                if (jsonHeader != null
                    || context.Request.Path.HasValue
                    && context.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                {
                    /*
                    var message = "Invalid User Credentials";
                    if (!string.IsNullOrEmpty(jsonHeader?.Message))
                        message = jsonHeader.Message;

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var jsonResponse = JsonSerializer.Serialize(new
                    {
                        error = message
                    });

                    await context.Response.WriteAsync(jsonResponse);
                    */

                    await context.Response.SendForbiddenAsync();
                    return;
                }
            }

            // Fallback to the default implementation.
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}

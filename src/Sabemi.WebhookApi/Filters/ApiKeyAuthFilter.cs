using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sabemi.WebhookApi.Filters;

public class ApiKeyAuthFilter : IAsyncActionFilter
{
    private const string HeaderName = "X-Api-Key";
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var chaveEsperada = _configuration["Security:ApiKey"];

        if (string.IsNullOrEmpty(chaveEsperada))
        {
            context.Result = new ObjectResult("ApiKey não configurada no servidor.")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var chaveRecebida) ||
            chaveRecebida != chaveEsperada)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                mensagem = "ApiKey ausente ou inválida."
            });
            return;
        }

        await next();
    }
}

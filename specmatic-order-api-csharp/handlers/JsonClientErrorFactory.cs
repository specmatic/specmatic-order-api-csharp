using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using specmatic_uuid_api.Models;

public class JsonClientErrorFactory : IClientErrorFactory
{
    public IActionResult GetClientError(ActionContext actionContext, IClientErrorActionResult clientError)
    {
        var statusCode = clientError.StatusCode ?? StatusCodes.Status500InternalServerError;
        var error = GetError(statusCode);

        var response = new ErrorResponse
        {
            TimeStamp = DateTime.UtcNow.ToString("o"),
            Error = error,
            Message = error
        };

        return new ObjectResult(response)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/json" }
        };
    }

    private static string GetError(int statusCode)
    {
        return ReasonPhrases.GetReasonPhrase(statusCode) is { Length: > 0 } reason ? reason : "Error";
    }
}

using System.Net;

namespace WebApplication9.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _request;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate request, ILogger<ExceptionMiddleware> logger)
    {
        _request = request;
        _logger = logger;

    }

    public async Task InvokeAsync(HttpContext httpcontext)
    {
        try
        {
            await _request(httpcontext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            await HandleExceptionAsync(httpcontext, ex); 
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        var response = new
        {
            StatusCode = context.Response.StatusCode, Message = "Bad Request, Please check your API", Detailed = exception.Message
        };
        return context.Response.WriteAsJsonAsync(response);
    }
}
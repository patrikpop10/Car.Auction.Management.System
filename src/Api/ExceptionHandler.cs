using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api;

public class ExceptionHandler : IExceptionHandler {
    private readonly ILogger<ExceptionHandler> _logger;
     public ExceptionHandler(ILogger<ExceptionHandler> logger) {
        // You can use the logger if needed
         _logger = logger;

    }
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {

        // Log the exception (you can use any logging framework)
        _logger.LogError(exception,"Exception occurred: {Message}", exception.Message);

        // Set the response status code and content
        httpContext.Response.StatusCode = 500; // Internal Server Error
        httpContext.Response.ContentType = "application/json";

        // Optionally, you can write a more detailed error response
        // use problem details
        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = 500,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        // Serialize the error response to JSON and write it to the response body
        return WriteErrorResponseAsync(httpContext, problemDetails, cancellationToken);
    }

    private ValueTask<bool> WriteErrorResponseAsync(HttpContext httpContext, ProblemDetails errorResponse, CancellationToken cancellationToken) {
        try {
            // Serialize the error response to JSON
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);

            // Write the JSON response to the response body
            httpContext.Response.WriteAsync(jsonResponse, cancellationToken);
            return ValueTask.FromResult(false);
        } catch (Exception ex) {
            // Log any errors that occur while writing the response
            _logger.LogError(ex, "Error writing error response");
            return ValueTask.FromResult(false);
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VideoCallService.Api.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the request
            await LogRequest(context);

            // Store the original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream to capture the response
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Continue processing the request
                await _next(context);

                // Log the response
                await LogResponse(context, responseBody, originalBodyStream);
            }
            finally
            {
                // Restore the original response body stream
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            var requestReader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var content = await requestReader.ReadToEndAsync();
            
            // Log the request details
            _logger.LogInformation(
                "HTTP {Method} {Path} received with query {Query} and body {Body}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                content);

            // Reset the request body position for the next middleware
            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
        {
            // Reset the memory stream position to read the response
            responseBody.Position = 0;

            var responseContent = await new StreamReader(responseBody, Encoding.UTF8).ReadToEndAsync();

            // Log the response details
            _logger.LogInformation(
                "HTTP {StatusCode} returned for {Method} {Path} with body {Body}",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path,
                responseContent);

            // Copy the response body to the original stream
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
} 
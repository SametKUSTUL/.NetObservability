using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System.Diagnostics;


namespace Common.Shared
{
    public class RequestAndResponseActivityMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public RequestAndResponseActivityMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await AddRequestBodyContentToActivityTag(context);
            await AddResponseBodyContentToActivityTag(context);

        }

        private async Task AddRequestBodyContentToActivityTag(HttpContext context)
        {
            context.Request.EnableBuffering();
            var requestBodyStreamReader = new StreamReader(context.Request.Body);
            var requestBody = await requestBodyStreamReader.ReadToEndAsync();
            Activity.Current?.SetTag("http.request.body", requestBody);
            context.Request.Body.Position = 0;
        }

        private async Task AddResponseBodyContentToActivityTag(HttpContext context)
        {

            var originalResponse = context.Response.Body;
            await using var responseBodyMemoryStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBodyMemoryStream;

            await _next(context);

            responseBodyMemoryStream.Position = 0;

            var responseBodyStreamReader = new StreamReader(responseBodyMemoryStream);
            var responseBody = await responseBodyStreamReader.ReadToEndAsync();
            Activity.Current?.SetTag("http.response.body", responseBody);

            responseBodyMemoryStream.Position = 0;
            await responseBodyMemoryStream.CopyToAsync(originalResponse);

        }

    }
}

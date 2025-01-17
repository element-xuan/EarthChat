﻿namespace Chat.Service.Infrastructure.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            if (context.Request.Path == "/")
            {
                context.Request.Path = "/index.html";
            }

            await next(context);

            if (context.Response.StatusCode == 404)
            {
                var webHost = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
                if (File.Exists(Path.Combine(webHost.WebRootPath, "index.html")))
                {
                    await using var file = File.OpenRead(Path.Combine(webHost.WebRootPath, "index.html"));
                    await file.CopyToAsync(context.Response.Body);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = 401;
        }
        catch (UserFriendlyException e)
        {
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(e.CreateExceptionResult("400"),
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(
                JsonSerializer.SerializeToUtf8Bytes(e.CreateExceptionResult("400"), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        }
    }
}
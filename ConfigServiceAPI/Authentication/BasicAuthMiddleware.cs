using System.Text;
using Microsoft.Extensions.Configuration;

namespace ConfigServiceAPI.Authentication
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string AuthorizationHeader = "Authorization";

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var config = context.RequestServices.GetRequiredService<IConfiguration>();

            if (!context.Request.Headers.TryGetValue(AuthorizationHeader, out var authHeader) ||
                !authHeader.ToString().StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"ConfigServiceAPI\"";
                await context.Response.WriteAsync("Autenticación requerida.");
                return;
            }

            var encoded = authHeader.ToString().Substring("Basic ".Length).Trim();
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var parts = decoded.Split(':');

            if (parts.Length != 2 || !IsAuthorized(parts[0], parts[1], config))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Credenciales inválidas.");
                return;
            }

            await _next(context);
        }

        private bool IsAuthorized(string username, string password, IConfiguration config)
        {
            string? ReadSecret(string path) =>
                File.Exists(path) ? File.ReadAllText(path).Trim() : null;

            var expectedUser = ReadSecret("/run/secrets/basic_auth_user") ?? config["BASIC_AUTH_USER"];
            var expectedPass = ReadSecret("/run/secrets/basic_auth_pass") ?? config["BASIC_AUTH_PASS"];

            return username == expectedUser && password == expectedPass;
        }

    }
}
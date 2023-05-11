using ContentSecurityPolicyWithSwagger.Extensions;
using System.Security.Cryptography;

namespace ContentSecurityPolicyWithSwagger.Middleware;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var nonce = GenerateNonce();
        httpContext.SetNonce(nonce);

        var contentSecurityPolicyValues = GenerateContentSecurityPolicyValues(nonce);
        httpContext.SetContentSecurityPolicy(contentSecurityPolicyValues);

        await _next(httpContext);
    }

    private static string GenerateNonce()
    {
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var nonceByteArray = new byte[64];
        randomNumberGenerator.GetBytes(nonceByteArray);
        return Convert.ToBase64String(nonceByteArray);
    }

    private static Dictionary<string, string> GenerateContentSecurityPolicyValues(string nonce)
    {
        var contentSecurityPolicyValues = new Dictionary<string, string>()
        {
            { "script-src", $"'self' 'nonce-{nonce}'" },
            { "style-src", $"'self' 'nonce-{nonce}'" }
        };

        return contentSecurityPolicyValues;
    }
}

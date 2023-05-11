using ContentSecurityPolicyWithSwagger.Utilities;
using System.Collections.Generic;

namespace ContentSecurityPolicyWithSwagger.Extensions;

public static class HttpContextExtensions
{
    public static string GetNonce(this HttpContext context) => context.Items[Constants.DefaultNonceKey] as string ?? string.Empty;

    public static void SetNonce(this HttpContext context, string nonce)
    {
        context.Items[Constants.DefaultNonceKey] =
            !string.IsNullOrEmpty(nonce)
                ? nonce
                : throw new ArgumentException("Value cannot be null or empty.", nameof(nonce));
    }

    public static void SetContentSecurityPolicy(this HttpContext context, Dictionary<string, string> contentSecurityPolicyValues)
    {
        if (contentSecurityPolicyValues.Count == 0)
        {
            throw new ArgumentException("Value cannot be empty.", nameof(contentSecurityPolicyValues));
        }

        context.Response.Headers.TryAdd(Constants.ContentSecurityPolicyHeaderName, string.Join("; ", contentSecurityPolicyValues.Select(v => $"{v.Key} {v.Value}")));
    }
}

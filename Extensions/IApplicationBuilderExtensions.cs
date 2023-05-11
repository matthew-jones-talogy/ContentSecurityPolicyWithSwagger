using ContentSecurityPolicyWithSwagger.Middleware;

namespace ContentSecurityPolicyWithSwagger.Extensions;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ContentSecurityPolicyMiddleware>();
    }
}

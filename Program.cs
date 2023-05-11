using ContentSecurityPolicyWithSwagger.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// https://mderriey.com/2020/12/14/how-to-lock-down-csp-using-swachbuckle/
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddOptions<SwaggerUIOptions>()
    .Configure<IHttpContextAccessor>((swaggerUiOptions, httpContextAccessor) =>
    {
        var originalIndexStreamFactory = swaggerUiOptions.IndexStream;

        swaggerUiOptions.IndexStream = () =>
        {
            using var originalStream = originalIndexStreamFactory();
            using var originalStreamReader = new StreamReader(originalStream);
            var originalIndexHtmlContents = originalStreamReader.ReadToEnd();

            var requestSpecificNonce = httpContextAccessor.HttpContext!.GetNonce();

            var nonceEnabledIndexHtmlContents = originalIndexHtmlContents
                .Replace("<script>", $"<script nonce=\"{requestSpecificNonce}\">", StringComparison.OrdinalIgnoreCase)
                .Replace("<style>", $"<style nonce=\"{requestSpecificNonce}\">", StringComparison.OrdinalIgnoreCase);

            return new MemoryStream(Encoding.UTF8.GetBytes(nonceEnabledIndexHtmlContents));
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseContentSecurityPolicy();

app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
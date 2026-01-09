using FluentAssertions;
using Jobify.Ecom.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Jobify.Ecom.Api.Tests.Services;

public class CookieServiceTests
{
    private readonly CookieService _cookieService;
    private readonly DefaultHttpContext _httpContext;

    public CookieServiceTests()
    {
        _cookieService = new CookieService();
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public void SetCookie_ShouldAppendCookieToResponse()
    {
        string name = "auth_token";
        string value = "xyz123";

        _cookieService.SetCookie(_httpContext.Response, name, value);

        StringValues headers = _httpContext.Response.Headers[HeaderNames.SetCookie];
        headers.Should().Contain(header => header.Contains($"{name}={value}"));
    }

    [Fact]
    public void GetCookie_ShouldRetrieveValueFromRequest()
    {
        string name = "auth_token";
        string value = "xyz123";
        string cookieHeader = $"{name}={value}";
        _httpContext.Request.Headers[HeaderNames.Cookie] = cookieHeader;

        string? result = _cookieService.GetCookie(_httpContext.Request, name);

        result.Should().Be(value);
    }

    [Fact]
    public void DeleteCookie_ShouldExpireCookie()
    {
        string name = "auth_token";

        _cookieService.DeleteCookie(_httpContext.Response, name);

        string headers = _httpContext.Response.Headers[HeaderNames.SetCookie][0] ?? string.Empty;

        headers.Should().Contain($"{name}=;");
        headers.Should().Contain("expires=Thu, 01 Jan 1970 00:00:00 GMT");
    }
}

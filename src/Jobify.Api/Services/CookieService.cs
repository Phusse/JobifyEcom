namespace Jobify.Api.Services;

internal class CookieService
{
    public static void SetCookie(HttpResponse response, string name, string value, DateTime? expiresUtc = null, bool httpOnly = true, bool secure = true, string path = "/", string? domain = null)
    {
        CookieOptions options = new()
        {
            HttpOnly = httpOnly,
            Secure = secure,
            SameSite = SameSiteMode.Strict,
            Expires = expiresUtc,
            Path = path,
        };

        if (!string.IsNullOrWhiteSpace(domain))
            options.Domain = domain;

        response.Cookies.Append(name, value, options);
    }

    public static string? GetCookie(HttpRequest request, string name)
        => request.Cookies.TryGetValue(name, out string? value) ? value : null;

    public static void DeleteCookie(HttpResponse response, string name, string path = "/", string? domain = null)
    {
        CookieOptions options = new()
        {
            Path = path,
            Expires = DateTime.UnixEpoch,
            SameSite = SameSiteMode.Strict,
        };

        if (!string.IsNullOrWhiteSpace(domain))
            options.Domain = domain;

        response.Cookies.Delete(name, options);
    }
}

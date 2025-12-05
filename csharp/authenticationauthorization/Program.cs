// Implementing ASP.NET identity and authentication by scratch.
// Hit login first, then username endpoint.

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CookieAuthService>();

// The AddAuthentication and UseAuthentication combo below, is the same as the uncommented app.Use middleware below.
// The AddCookie method gives the handler that will do the WhatAddCookieDoesWhenGettingARequest region in the middleware
// and the WhatAddCookieDoesToCreateACookie region.
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
var app = builder.Build();
//app.UseAuthentication();

app.Use((ctx, next) =>
{
    // Middleware to decode a cookie; demoing how ASP.NET framework would do it
    
    // For demo, unprotect only login endpoint
    if (ctx.Request.Path.ToString().Contains("login"))
        return next();
    
    # region WhatAddCookieDoesWhenGettingARequest
    // Get the ASP.NET data protection toolkit
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    // Get the relevant protector (i.e. the one with the correct protection purpose).
    // In microservices, usually the purpose is shared in a shared folder/other ways across the services
    var protector = idp.CreateProtector(CookieConstants.CookieProtectionPurpose);
    // All cookies in the browser are returned in the format: cookie1=value1; cookie2=value2; ... Decode them and get
    // only the relevant one
    var authCookieValue = ctx.Request.Headers.Cookie.First().Split(";").First(x => x.Trim().StartsWith(CookieConstants.AuthCookieKey)).Split("=").Last();
    // Unprotect the cookie
    var json = protector.Unprotect(authCookieValue);
    var unprotectedClaimData = JsonSerializer.Deserialize<List<ClaimData>>(json);
    # endregion
    
    // An collection of claims is called an identity.
    // As said before, this collection of claims (i.e. the identity) is 'issued' or 'defined' or 'created' by an 'issuer' or 'authenticator' or
    // 'authentication scheme' or 'scheme'; these are the same thing.
    // A ClaimsIdentity is just a class holding the combo of an authentication scheme and the identity it issues.
    // Example of authentication system/identity providers that are common:
    // "Cookies", "Bearer", "oidc", "Google", "Negotiate", "MyCustomScheme".
    // CookieAuthenticationDefaults.AuthenticationScheme below = "Cookies".
    ClaimsIdentity identity = new ClaimsIdentity(unprotectedClaimData.Select(c => new Claim(c.Type, c.Value)), CookieAuthenticationDefaults.AuthenticationScheme);
    // A ClaimsPrincipal is a collection of identities.
    // A person/user is represented by a ClaimsPrincipal
    ClaimsPrincipal user = new ClaimsPrincipal([identity]);
    // Attach the user to the request's User
    ctx.User = user;
    // Continue the middleware chain
    return next();
});
app.MapGet("/username", (HttpContext request) =>
{
    // User has 3 public properties: claims, identities, and identity
    // Identity is basically the first identity the framework can find in the list of identities.
    // In the below, we return all claims inside all identities
    return request.User.Claims.Select(x => x.Type + " " + x.Value);
});

app.MapGet("/login", (HttpContext ctx, CookieAuthService cookieCookieAuthService) =>
{
    // Cookie-based authentication scheme
    
    // 1. Validate login credentials
    if (!cookieCookieAuthService.ValidateLogin())
        throw new BadHttpRequestException("you are not validated");
    
    // After validating, usually we read from the database, get all the claims and data to be serialized into the cookie.
    // This is faked with new instantiations below.
    // The claims and data is put into the cookie, so that, when other microservices read the cookie, they know who you
    // are and your claims, and can
    
    // 2. Construct claims after successful validation
    // A claim is a piece of information about a user (or entity) that has been issued by an identity provider or
    // authentication system. In .NET, a claim is represented by the Claim class.
    // Example below: a Role claim and an Email claim
    List<Claim> claims = new List<Claim>();
    claims.Add(new Claim(ClaimTypes.Role, "some-role-issued-by-some-authentication-scheme"));
    claims.Add(new Claim(ClaimTypes.Email, "some@email.com"));
    // An collection of claims is called an identity.
    // As said before, this collection of claims (i.e. the identity) is 'issued' or 'defined' or 'created' by an 'issuer' or 'authenticator' or
    // 'authentication scheme' or 'scheme'; these are the same thing.
    // A ClaimsIdentity is just a class holding the combo of an authentication scheme and the identity it issues.
    // Example of authentication system/identity providers that are common:
    // "Cookies", "Bearer", "oidc", "Google", "Negotiate", "MyCustomAuthenticatorScheme".
    // CookieAuthenticationDefaults.AuthenticationScheme constant below is "Cookies".
    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    // A ClaimsPrincipal is a collection of identities.
    // A person/user is represented by a ClaimsPrincipal
    ClaimsPrincipal user = new ClaimsPrincipal([identity]);
    
    // 3. Construct the cookie and append it to the request
    cookieCookieAuthService.SignIn(user);
    // The SignIn method above is roughly equivalent to the framework code:
    // ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user).GetAwaiter().GetResult();
    // This method btw is more inclusive, since it will also include the Issuer information in the cookie
    return "Login okay! check your cookie in F12.";
});

app.Run();

public class CookieAuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
{
    public bool ValidateLogin()
    {
        return true; // Fake login validation, always return true for example sake
    }
    
    public void SignIn(ClaimsPrincipal user)
    {
        // Roughly what the ASP.NET SignInAsync method does
        # region WhatAddCookieDoesToCreateACookie
        // 1. Create a protector (a way to hash/encrypt i.e. make the cookie secure)
        var protector = idp.CreateProtector(CookieConstants.CookieProtectionPurpose);
        // 2. Serialize claims to JSON; usually it is a key value pair of the type and the value
        var claimsData = user.Claims.Select(c => new ClaimData() { Type = c.Type, Value = c.Value }).ToList();
        var json = JsonSerializer.Serialize<List<ClaimData>>(claimsData);
        // 3. Set the cookie to the request
        accessor.HttpContext.Response.Headers["set-cookie"] = $"{CookieConstants.AuthCookieKey}={protector.Protect(json)}";
        # endregion
    }
}

# region enums
public static class CookieConstants
{
    public static string AuthCookieKey = "some-cookie-containing-claims";
    public static string CookieProtectionPurpose = "cookie-protection-purpose";
}

public class ClaimData
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
# endregion
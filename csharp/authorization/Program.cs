using System.Collections;
using System.ComponentModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Swashbuckle.AspNetCore.Annotations;

// Study first authentication notes to understand how this authorization works.
// Authentication means check if the cookie is valid (through DataProtector()).
// Authorization means check the authentication scheme and the claims in the cookie, whether it matches the rules
// i.e. "policy" of the endpoint to be accessed.

var builder = WebApplication.CreateBuilder(args);

// --------------------------
//  ADD SERVICES
// --------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication middleware.
// Rememeber: a scheme is like the user choosing to login using OIDC, Local auth etc. in the frontend.
// Here we show that even with one Local authentication method (cookie based), we can further break it into multiple
// authentication schemes.
// So although in tutorials online it seems one authentication method equals one scheme, and this impression is also reinforced
// with classes like CookieAuthenticationDefaults.CookiePrefix, this is not the case! We can have multiple schemes for the
// same authentication method.

builder.Services.AddAuthentication()  // Since not specified, the default is customer scheme (the first one in the chain)
    .AddCookie(AuthSchemes.CustomerScheme, options =>
    {
        options.Cookie.Name = "Customer.Auth";
        options.LoginPath = "/login";  // Endpoint to redirect unauthenticated users to
    })
    .AddCookie(AuthSchemes.EmployeeScheme, options =>
    {
        options.Cookie.Name = "Employee.Auth";
        options.LoginPath = "/login";  // Endpoint to redirect unauthenticated users to
    });

// Authorization middleware
// Define policies, that can be attached to different endpoints
// Use the names in the endpoints to protect (see endpoints)
builder.Services.AddAuthorization(bp =>
{
    bp.AddPolicy("Only admins", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthSchemes.EmployeeScheme)
            .RequireClaim(ClaimTypes.Role, EmployeeRoles.Admin);
    });
    bp.AddPolicy("Only VIPs", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthSchemes.CustomerScheme)
            .RequireClaim(ClaimTypes.Role, CustomerRoles.VIPCustomer);
    });
});

var app = builder.Build();

// --------------------------
//  SWAGGER
// --------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// --------------------------
//  POST /login
// --------------------------
app.MapPost("/login", async (HttpContext context, LoginRequest request) =>
{
    // Determine scheme
    string? scheme = request.Schema?.ToLower() switch
    {
        "customer" => AuthSchemes.CustomerScheme,
        "employee" => AuthSchemes.EmployeeScheme,
        _ => null
    };

    if (scheme == null)
        return Results.BadRequest("Unknown schema. Use 'customer' or 'employee'.");

    // Validate credentials
    if (!AuthService.ValidateCredentials(request.Username, request.Password))
        return Results.Unauthorized();

    // Claims
    // Build and attach the correct schemes based on the credentials of the user logging in
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, request.Username),
    };
    
    if (scheme == AuthSchemes.EmployeeScheme)
        if (request.Username == Usernames.CompanyFounder)
            claims.Add(new Claim(ClaimTypes.Role, EmployeeRoles.Admin));
        else if (request.Username == Usernames.SomeProgrammer || request.Username == Usernames.SomeSalesman)
            claims.Add(new Claim(ClaimTypes.Role, EmployeeRoles.Staff));
        else
            throw new BadHttpRequestException("Can't login with this scheme for this username");
    else
        if (request.Username == Usernames.VIPCustomer)
            claims.Add(new Claim(ClaimTypes.Role, CustomerRoles.VIPCustomer));
        else if (request.Username == Usernames.RegularCustomer)
            claims.Add(new Claim(ClaimTypes.Role, CustomerRoles.RegularCustomer));
        else
            throw new BadHttpRequestException("Can't login with this scheme for this username");
    

    // Sign-in cookie
    var identity = new ClaimsIdentity(claims, scheme);
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(scheme, principal);

    return Results.Ok(new { message = "Logged in", scheme });
})
.WithTags("Auth")
.WithSummary($"Login using customer or employee scheme ")
.WithDescription($"Send username, password, schema  and receive an authentication cookie, pass is 123. Usernames possible: {string.Join(", ", Usernames.AllUsernames())}");

// --------------------------
//  PROTECTED ENDPOINTS
// --------------------------

app.MapGet("/customer-area", () => "Welcome Customer (doesn't matter if you are VIP or not)!")
   .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = AuthSchemes.CustomerScheme })
   .WithTags("Protected");

app.MapGet("/vip-area", () => "Welcome VIP customer!")
    .RequireAuthorization("Only VIPs")
    .WithTags("Protected");

app.MapGet("/employee-area", () => "Welcome Employee (doesn't matter if you are admin or just regular)!")
   .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = AuthSchemes.EmployeeScheme })
   .WithTags("Protected");

app.MapGet("/admin-area", () => "Welcome Admin!")
    .RequireAuthorization("Only admins")
    .WithTags("Protected");

// --------------------------
//  RUN
// --------------------------
app.Run();

# region classes
// --------------------------
//  AUTH SCHEMES
// --------------------------
public static class AuthSchemes
{
    public const string CustomerScheme = "CustomerScheme";
    public const string EmployeeScheme = "EmployeeScheme";
}

// --------------------------
//  REQUEST MODEL
// --------------------------
public class LoginRequest
{
    [DefaultValue("RegularCustomer")]
    public string Username { get; set; }
    [DefaultValue("123")]
    public string Password { get; set; }
    [DefaultValue("employee")]
    public string Schema { get; set; } // "customer" or "employee"
}

// Fake AuthService for demo purposes
class Usernames : IEnumerable<string>
{
    public  static string RegularCustomer = nameof(RegularCustomer);
    public static string VIPCustomer = nameof(VIPCustomer);
    public  static string CompanyFounder = nameof(CompanyFounder);
    public  static string SomeProgrammer = nameof(SomeProgrammer);
    public static string SomeSalesman = nameof(SomeSalesman);

    public IEnumerator<string> GetEnumerator()
    {
        yield return RegularCustomer;
        yield return VIPCustomer;
        yield return CompanyFounder;
        yield return SomeProgrammer;
        yield return SomeSalesman;
    }

    IEnumerator IEnumerable.GetEnumerator() =>GetEnumerator();
    public static IEnumerable<string> AllUsernames()
    {
        var t = new Usernames();
        return t.ToList();
    }
}
static class AuthService
{
    public static bool ValidateCredentials(string username, string password)
    {
        var usernames = new Usernames(); 
        return usernames.Contains(username) && password == "123";
    }
}

static class EmployeeRoles
{
    public static string Admin = "Admin";
    public static string Staff = "Staff";
}

static class CustomerRoles
{
    public static string VIPCustomer = "VIP";
    public static string RegularCustomer = "Regular";
}

# endregion
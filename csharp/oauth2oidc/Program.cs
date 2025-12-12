using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

// Summary:
// This toy is a barebones oidc implementation, to experiment with how oauth2 and oidc works instead of relying on
// blackbox libraries and not understanding anything on how it actually works and just hacking/trial erroring your way
// through it.

// Make breakpoints and follow along for learning.

// Setup on google side:
// First, using any google account, register this app in google console of that account, and get clientid and secret from there and register it in the appsettings.json
// Second, the app will be in testing phase. Go to audience tab, and in the bottom of the page, add the emails of the google accounts you will be using to access the app for playing around
// If you don't do this, Google will complain during redirection (see below)

// The oidc flow is:
// 1. User goes to /login, gets redirected to Auth server (in this example google)
// 2. In the redirect page, look at the URL. There is a lot of stuff, example:
// https://accounts.google.com/v3/signin/accountchooser?client_id=733422730315-e5d06ml6au2kh0j8tcvalc1kp6nmaith.apps.googleusercontent.com
//                                          &code_challenge=GG9r39x4lUB7qOYdx728hE1O40dlgnyjlfXOiabMjqw
//                                          &code_challenge_method=S256
//                                          &nonce=SFlIGjW-AddwwRZV0yToAIxya2fr7EGi7dTMSzSlUG4
//                                          &redirect_uri=http%3A%2F%2Flocalhost%3A5000%2Fsigninoidc
//                                          &response_type=code
//                                          &scope=openid+profile+email+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar.readonly+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive.metadata.readonly
//                                          &state=S8wxHL7yGXkCmXl9OJmAIOHAlVAR5-gGm2NfO3i_Lfk
//                                          &dsh=S1108083311%3A1765581494165496
//                                          &o2v=2
//                                          &service=lso
//                                          &flowName=GeneralOAuthFlow
//                                          &opparams=%253F
//                                          &continue=https%3A%2F%2Faccounts.google.com%2Fsignin%2Foauth%2Fconsent%3Fauthuser%3Dunknown%26part%3DAJi8hAMVjcaTgHD4715cG80A7LqbvBlvlAwiaZVEPYEeRO05Qj_cQLuMFuc2tkUo9A5rWwWu3hYYloiInk2fOhDBWjuPNZhlUEi2IPH6ZMpCOg_scw54tTqWsF5RYFxfP9GswW3i6ZEpabupkQeA6S8ppNpJxRMmb5UCjhl8tDHIC1X9EIIIOPvBiJA245aKAtT-yBXWsV3pvHql1Gk42MdzFlmZe7gTFNbablP_0wAYF2wTH20cUJ-xBrHdOoNQNaZFUP9FWB2M0CFdwkSvWImaetTfe_BVZX7YEYTPCGa5QhgavnYquTxYbQ8CbAXG__81t8_oyn67GDiptWFUkSebt7SBg_ZMRI_zIRd605J6gwnJtFztbhp0wcBORI6KMo1UI8yNUbwXtVY0OYf0X_8JLeDHn2O3FUdapPdFkz_XjTSEzwgZ6BGJqeCkMWIlOQzY3ElKvfQvTEEactqEmrGqb3tRJOFYE7s3WqPqTYTbp1ZY-Y9ToTc%26flowName%3DGeneralOAuthFlow%26as%3DS1108083311%253A1765581494165496%26client_id%3D733422730315-e5d06ml6au2kh0j8tcvalc1kp6nmaith.apps.googleusercontent.com%26requestPath%3D%252Fsignin%252Foauth%252Fconsent%23
//                                          &app_domain=http%3A%2F%2Flocalhost%3A5000
// If you go to /login implementation below, it will be explained what each are.
// 3. Login with google acc. If your account email has the string defined in IamAldiSpecialUser variable below, then you can access a special endpoint guarded with an auth policy only for users with a special claim.
// See implementation for details. In real life this can be e.g. if user email is from e.g. yahoo.com, add ClaimTypes.Role = yahooUsers, and guard some endpoints based on this rule.
// This contrived example is to illustrate how the oidc flow returns the user details in the google side (which is not the case if we only use oauth2), and we can use it to add custom claims like this, or perhaps register the user to our own DB (remember 3mensio) and add roles specific to our app, etc.
// 4. The browser will go to &continue in the above link, ask if the user wants to give consent for the app to access the scopes requested (e.g. access to the user's google drive data, calendar, etc). If so, then google will redirect us to the &redirect_uri 
// 5. Go to the redirect uri endpoint implementation below, to study what is going on.
// 6. Once successful, in browser you will be redirected to some page. F12 and check cookies, you will see the auth cookie. You are authenticated, and the cookie inside contains all your claims set during the redirect_uri implementation.
// 7. Go to some endpoint we have below that accesses e.g. your calendar info, gdrive info. This backend will use access_token given by google during the redirect_uri implementation, and request the data from google.

string IamAldiSpecialUser = "alditerawam";
string SpecialUserClaimKey = nameof(SpecialUserClaimKey);
string SpecialUserClaimValue = nameof(SpecialUserClaimValue);
string SpecialUserAuthPolicy = "OnlyUserWithSpecialUserClaim";

var builder = WebApplication.CreateBuilder(args);

// In-memory store for demo only — use a persistent/sealed store in real apps
var transientStore = new Dictionary<string, (string CodeVerifier, string Nonce, DateTimeOffset Expiry)>();

// Configuration - change these for your provider / app
var oidcConfig = builder.Configuration.GetSection("OpenIDConnectSettings");
var oidcAuthority = oidcConfig["Authority"]; // example: Google
var clientId = oidcConfig["ClientId"];
var clientSecret = oidcConfig["ClientSecret"]; // for confidential clients
var redirectUri = "http://localhost:5000/signinoidc"; // must match registered redirect in authority side, go to google console where you registered your app, and add this

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "oauth2oidctoyproject.Auth";  // After successful login, F12 and check it out that your auth cookie has this name
        options.LoginPath = "/login"; // Endpoint to redirect unauthenticated users to
    });
builder.Services.AddAuthorization(bp =>
{
    bp.AddPolicy(SpecialUserAuthPolicy, pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireClaim(SpecialUserClaimKey, SpecialUserClaimValue);
    });
});
// .AddOpenIdConnect(options =>
// {
//     // ........................................................................
//     // The OIDC handler must use a sign-in scheme capable of persisting 
//     // user credentials across requests.
//     
//     options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     // ........................................................................
//
//     // ........................................................................
//     // The "openid" and "profile" scopes are required for the OIDC handler 
//     // and included by default. 
//
//     //options.Scope.Add("some-scope");
//     // ........................................................................
//
//     // ........................................................................
//     // The following paths must match the redirect and post logout redirect 
//     // paths configured when registering the application with the OIDC provider. 
//     // Both the signin and signout paths must be registered as Redirect URIs.
//     // The default values are "/signin-oidc" and "/signout-callback-oidc".
//
//     //options.CallbackPath = new PathString("/signin-oidc");
//     //options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
//     // ........................................................................
//
//     // ........................................................................
//     // The RemoteSignOutPath is the "Front-channel logout URL" for remote single 
//     // sign-out. The default value is "/signout-oidc".
//
//     //options.RemoteSignOutPath = new PathString("/signout-oidc");
//     // ........................................................................
//
//     var oidcConfig = builder.Configuration.GetSection("OpenIDConnectSettings");
//     // ........................................................................
//     // Authority is the OIDC provider's base URL. Set the application settings
//
//     options.Authority = oidcConfig["Authority"];
//     // ........................................................................
//
//     // ........................................................................
//     // Set the Client ID for the app. Set the application settings to
//     // the Client ID.
//
//     options.ClientId = oidcConfig["ClientId"];
//     // ........................................................................
//
//
//     options.ClientSecret = oidcConfig["ClientSecret"];
//
//     // ........................................................................
//     // Setting ResponseType to "code" configures the OIDC handler to use 
//     // authorization code flow. The OIDC handler automatically requests the
//     // appropriate tokens using the code returned from the
//     // authorization endpoint.
//
//     options.ResponseType = OpenIdConnectResponseType.Code;
//     // ........................................................................
//
//     // ........................................................................
//     // Set MapInboundClaims to "false" to obtain the original claim types from 
//     // the token. Many OIDC servers use "name" and "role"/"roles" rather than 
//     // the SOAP/WS-Fed defaults in ClaimTypes. Adjust these values if your 
//     // identity provider uses different claim types.
//
//     options.MapInboundClaims = false;
//     options.TokenValidationParameters.NameClaimType = "name";
//     options.TokenValidationParameters.RoleClaimType = "role";
//     // ........................................................................
//
//     options.SaveTokens = true;
//     options.GetClaimsFromUserInfoEndpoint = true;
// });

// rewrite this to use lambda

var app = builder.Build();

// Discover OIDC endpoints and jwks (simple cache)
using var someHttpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
var discovery = await FetchDiscoveryAsync(someHttpClient, oidcAuthority);

app.UseAuthentication();
app.UseAuthorization();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();


// =================== Endpoints ===================

app.MapGet("/login", async (HttpContext ctx) =>
{
    // Generate cryptographically random values
    // TODO: add explanation what these are for
    var state = RandomBase64Url(32);
    var nonce = RandomBase64Url(32);
    var codeVerifier = RandomBase64Url(64);
    var codeChallenge = ComputeCodeChallenge(codeVerifier); // SHA256 + base64url

    // Save verifier & nonce keyed by state (short lived)
    transientStore[state] = (CodeVerifier: codeVerifier, Nonce: nonce, Expiry: DateTimeOffset.UtcNow.AddMinutes(5));

    var authorizeUrl = QueryHelpers.AddQueryString(discovery.AuthorizationEndpoint, new Dictionary<string, string?>
    {
        ["client_id"] = clientId,
        ["response_type"] = "code",  // TODO: Why code?
        // Here since we explicitly ask openid scope, the request becomes oidc flow (so user details will be included in response).
        // If oauth2 is set instead or no openid or oauth2 is set, then it is defaulted to oauth2.
        ["scope"] = "openid profile email " + 
                    "https://www.googleapis.com/auth/gmail.readonly " +
                    "https://www.googleapis.com/auth/drive.metadata.readonly",
        ["redirect_uri"] = redirectUri,
        ["state"] = state,
        ["nonce"] = nonce,
        ["code_challenge"] = codeChallenge,
        ["code_challenge_method"] = "S256"
    });
    
    ctx.Response.Redirect(authorizeUrl);
});

// -- 2) Callback endpoint: After user consents, google will redirect to this endpoint.
// exchange code for tokens, validate id_token, validate user creds, and finally sign in
app.MapGet("/signinoidc", async (HttpContext ctx) =>
{
    var q = ctx.Request.Query;
    var state = q["state"].FirstOrDefault();
    var code = q["code"].FirstOrDefault();
    var error = q["error"].FirstOrDefault();

    if (!string.IsNullOrEmpty(error))
    {
        await ctx.Response.WriteAsync($"OIDC error: {error}");
        return;
    }

    if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code))
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsync("Missing state or code.");
        return;
    }

    if (!transientStore.TryGetValue(state, out var entry) || entry.Expiry < DateTimeOffset.UtcNow)
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsync("Invalid or expired state.");
        return;
    }

    // Remove used state (single-use)
    transientStore.Remove(state);
    var codeVerifier = entry.CodeVerifier;
    var expectedNonce = entry.Nonce;

    // Exchange code for tokens
    var tokenResponse = await ExchangeCodeForTokensAsync(someHttpClient, discovery.TokenEndpoint, new Dictionary<string, string?>
    {
        ["grant_type"] = "authorization_code",
        ["code"] = code,
        ["redirect_uri"] = redirectUri,
        ["client_id"] = clientId,
        // If your client is confidential, include client_secret:
        ["client_secret"] = clientSecret,
        ["code_verifier"] = codeVerifier
    });

    if (tokenResponse == null)
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsync("Token exchange failed.");
        return;
    }

    // tokenResponse should contain id_token, access_token, refresh_token (maybe)
    var idToken = tokenResponse.Value.GetProperty("id_token").GetString();
    var accessToken = tokenResponse.Value.TryGetProperty("access_token", out var a) ? a.GetString() : null;
    var refreshToken = tokenResponse.Value.TryGetProperty("refresh_token", out var r) ? r.GetString() : null;

    // Validate id_token signature & claims
    var principal = await ValidateIdTokenAsync(idToken, discovery.JwksUri, clientId, expectedNonce, discovery.Issuer, someHttpClient);
    
    // This is where you would, based on the principal values, validate if the user is valid from your app side, e.g.:
    // 1. Look up the user in your local db and validate they exist/make a local account for them if not exist
    // 2. Add custom claims/roles/whatever based on the values in their client ID
    // Here we just simply make a weird rule: if the user email obtained from oidc contains the alditerawam string,
    // then add a custom claim.
    // Look at the auth policy too in builder above; that guards endpoints only for those with this claim in their cookie
    if (principal.FindFirstValue(ClaimTypes.Email).Contains(IamAldiSpecialUser))
        principal.Identities.First().AddClaim(new Claim(SpecialUserClaimKey, SpecialUserClaimValue));
    
    if (principal == null)
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("Invalid ID token.");
        return;
    }

    // Optionally store tokens in auth properties.
    // This is so you can make endpoints to access the user's google email, etc. using e.g. the access token
    var props = new AuthenticationProperties(new Dictionary<string, string?>
    {
        [".Token.access_token"] = accessToken,
        [".Token.refresh_token"] = refreshToken,
        [".Token.id_token"] = idToken
    })
    {
        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
    };
    
    // Finally sign in the user, after you validate above the user is valid (based on their google profile data)
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

    ctx.Response.Redirect("/");
});

app.MapGet("/", async (HttpContext ctx) =>
{
    return $"hello, go to /login and login using a google account. If your email has the string {IamAldiSpecialUser} you can access /onlyforuserwithspecialclaim endpoint.";
});

app.MapGet("/onlyforuserwithspecialclaim", async (HttpContext ctx) =>
{
    return $"hello you are a special user {IamAldiSpecialUser} accessing endpoint special for you";
}).RequireAuthorization(SpecialUserAuthPolicy);

app.MapGet("/calendar/events", async (HttpContext ctx) =>
{
    var token = await ctx.GetTokenAsync("access_token");

    using var request = new HttpRequestMessage(
        HttpMethod.Get,
        "https://www.googleapis.com/calendar/v3/users/me/calendarList"
    );

    request.Headers.Authorization =
        new AuthenticationHeaderValue("Bearer", token);

    var response = await someHttpClient.SendAsync(request);
    var data = await response.Content.ReadAsStringAsync();
    return data;
});

// TODO: Add e.g. youtube, others, but ofc add the scopes first!

app.Run();














   
// =================== Helper functions ===================

static string RandomBase64Url(int byteLength)
{
    var bytes = RandomNumberGenerator.GetBytes(byteLength);
    return Base64UrlEncode(bytes);
}

static string ComputeCodeChallenge(string codeVerifier)
{
    var bytes = Encoding.ASCII.GetBytes(codeVerifier);
    using var sha256 = SHA256.Create();
    var hash = sha256.ComputeHash(bytes);
    return Base64UrlEncode(hash);
}

static string Base64UrlEncode(byte[] input)
{
    return Convert.ToBase64String(input)
        .TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

static async Task<DiscoveryDoc> FetchDiscoveryAsync(HttpClient http, string issuerBase)
{
    // Standard location: {issuer}/.well-known/openid-configuration
    var discoUrl = issuerBase.TrimEnd('/') + "/.well-known/openid-configuration";  
    var resp = await http.GetAsync(discoUrl);
    resp.EnsureSuccessStatusCode();
    using var stream = await resp.Content.ReadAsStreamAsync();
    var doc = await JsonDocument.ParseAsync(stream);
    var auth = doc.RootElement.GetProperty("authorization_endpoint").GetString()!;
    var token = doc.RootElement.GetProperty("token_endpoint").GetString()!;
    var jwks = doc.RootElement.GetProperty("jwks_uri").GetString()!;
    var issuer = doc.RootElement.GetProperty("issuer").GetString()!;
    return new DiscoveryDoc(auth, token, jwks, issuer);
}

static async Task<JsonElement?> ExchangeCodeForTokensAsync(HttpClient http, string tokenEndpoint, Dictionary<string, string?> values)
{
    var content = new FormUrlEncodedContent(values.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value!));
    var resp = await http.PostAsync(tokenEndpoint, content);
    if (!resp.IsSuccessStatusCode) return null;
    var s = await resp.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(s);
    return doc.RootElement;
}

static async Task<System.Security.Claims.ClaimsPrincipal?> ValidateIdTokenAsync(string idToken, string jwksUri, string expectedAudience, string expectedNonce, string expectedIssuer, HttpClient http)
{
    // Fetch JWKS
    var jwksJson = await http.GetStringAsync(jwksUri);
    using var jwksDoc = JsonDocument.Parse(jwksJson);
    var keys = new List<SecurityKey>();
    foreach (var k in jwksDoc.RootElement.GetProperty("keys").EnumerateArray())
    {
        // This is a minimal parser for RSA keys. For production prefer Microsoft.IdentityModel.Protocols.OpenIdConnect
        var e = Base64UrlDecode(k.GetProperty("e").GetString()!);
        var n = Base64UrlDecode(k.GetProperty("n").GetString()!);
        var rsa = new System.Security.Cryptography.RSAParameters { Exponent = e, Modulus = n };
        var rsaKey = new RsaSecurityKey(rsa) { KeyId = k.GetProperty("kid").GetString() };
        keys.Add(rsaKey);
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var validationParameters = new TokenValidationParameters
    {
        ValidIssuer = expectedIssuer,
        ValidAudience = expectedAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = keys,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2),
        RequireSignedTokens = true
    };

    try
    {
        var principal = tokenHandler.ValidateToken(idToken, validationParameters, out var validatedToken);

        // Validate nonce claim
        var nonceClaim = principal.Claims.FirstOrDefault(c => c.Type == "nonce")?.Value;
        if (nonceClaim == null || nonceClaim != expectedNonce)
            return null;

        // Optionally check at_hash, acr, etc.

        return principal;
    }
    catch (Exception ex)
    {
        Console.WriteLine("ID token validation failed: " + ex);
        return null;
    }
}

// helper decode base64url to byte[]
static byte[] Base64UrlDecode(string input)
{
    string s = input.Replace('-', '+').Replace('_', '/');
    switch (s.Length % 4)
    {
        case 2: s += "=="; break;
        case 3: s += "="; break;
    }
    return Convert.FromBase64String(s);
}

record DiscoveryDoc(string AuthorizationEndpoint, string TokenEndpoint, string JwksUri, string Issuer);

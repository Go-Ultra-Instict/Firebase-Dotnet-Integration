using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Dev.TestMate.WebAPI.Authorization;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string BEARER_PREFIX = "Bearer ";
    private readonly FirebaseApp _firebaseApp;

    public FirebaseAuthenticationHandler(FirebaseApp firebaseApp, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _firebaseApp = firebaseApp;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {


        // removes a "Bearer" prefix, if sent
        string? token = Context.Request.Headers["Authorization"]
            .FirstOrDefault()
            ?.Split(" ")
            .Last();
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(
            token
        );

        Context.Items["User"] = decodedToken;
        //if (Context.Request.Headers.ContainsKey("Authorization"))
        //{
        //    return AuthenticateResult.NoResult();
        //}

        //string bearerToken = Context.Request.Headers["Authorization"];

        //if (bearerToken == null || !bearerToken.StartsWith(BEARER_PREFIX))
        //{
        //    return AuthenticateResult.Fail("Invalid scheme.");
        //}

        //string token = bearerToken.Substring(BEARER_PREFIX.Length);

        //try
        //{
        //    FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);

        //    return AuthenticateResult.Success(CreateAuthenticationTicket(firebaseToken));
        //}
        //catch (Exception ex)
        //{
        //    return AuthenticateResult.Fail(ex);
        //}
        return null;
    }

    private AuthenticationTicket CreateAuthenticationTicket(FirebaseToken firebaseToken)
    {
        var x = nameof(ClaimsIdentity);
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(ClaimsIdentity))
            });

        return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
    }

    private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
    {
        return new List<Claim>
            {
                new Claim("ID", claims.GetValueOrDefault("user_id", "").ToString()),
        };
    }
}


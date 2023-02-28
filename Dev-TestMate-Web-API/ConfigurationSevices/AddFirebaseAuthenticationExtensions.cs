using Dev.TestMate.WebAPI.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Dev.TestMate.WebAPI.ConfigurationSevices;

public static class AddFirebaseAuthenticationExtensions
{
    public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>
            (JwtBearerDefaults.AuthenticationScheme,
            (o) => { }
            );

        services.AddScoped<FirebaseAuthenticationHandler>();

        return services;
    }
}


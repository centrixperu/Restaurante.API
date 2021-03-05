using Centrix.Encore.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Centrix.Encore.Extensions
{
    public static class JWTExtension
    {
        public static void AddJWTExtesion(this IServiceCollection services, IConfiguration configuration, string authenticateScheme)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSetting>();

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JWTConfigurations.Secret));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = appSettings.JWTConfigurations.Iss,
                ValidateAudience = true,
                ValidAudience = appSettings.JWTConfigurations.Aud,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = authenticateScheme;
            })
           .AddJwtBearer(authenticateScheme, x =>
           {
               x.RequireHttpsMetadata = false;
               x.TokenValidationParameters = tokenValidationParameters;
           });
        }
    }
}

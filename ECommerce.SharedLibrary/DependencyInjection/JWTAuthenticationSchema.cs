using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ECommerce.SharedLibrary.DependencyInjection
{
   public static class JWTAuthenticationSchema
   {
      public static IServiceCollection AddJWTAuthenticationSchema(this IServiceCollection services, IConfiguration config)
      {
         //Add JWT Service
         services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", options =>
            {
               var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
               string issuer = config.GetSection("Authentication:Issuer").Value!;
               string audience = config.GetSection("Authentication:Audience").Value!;

               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = false,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = issuer,
                  ValidAudience = audience,
                  IssuerSigningKey = new SymmetricSecurityKey(key)
               };
            });
         return services;
      }
   }
}

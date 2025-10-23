using System;
using System.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;

[assembly: OwinStartup(typeof(SmartShift.Ui.Api.Startup))]

namespace SmartShift.Ui.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureJwtAuth(app);
        }

        private void ConfigureJwtAuth(IAppBuilder app)
        {
            var secret = ConfigurationManager.AppSettings["JwtSecret"];
            if (string.IsNullOrEmpty(secret))
                throw new Exception("JwtSecret not configured in Web.config <appSettings>.");

            var key = Encoding.ASCII.GetBytes(secret);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero 
                }
            });
        }
    }
}

using Core.Entities.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppIdentityDbContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("IdentityConnection"));
            });

            services.AddIdentityCore<AppUser>(opt =>
            {
                //add identity options here
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddSignInManager<SignInManager<AppUser>>(); //sign in manager daje vise opcija od UserManagera

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Auhth enticatioin dolazi prije jer nemoze Autorizitati nekog da nesto radi prije nego provjerimo ko je
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
                    ValidIssuer = config["Token:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}

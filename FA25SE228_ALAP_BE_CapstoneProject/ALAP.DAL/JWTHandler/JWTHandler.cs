using System.Security.Claims;
using System.Text;
using App.Entity.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
namespace EventZ.API.MiddleWare
{
    public static class JWTHandler
    {
        public static string GenerateJWT(this UserModel user, IConfiguration _configguration)
        {
            var secretKey = _configguration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            //if (user.Role != null && user.UserRoles.Any())
            //{
            //    foreach (var role in user.UserRoles.Select(ur => ur.Role.Name))
            //    {
            //        claims.Add(new Claim(ClaimTypes.Role, role));
            //    }
            //}

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public static int GetUserIdFromHttpContext(HttpContext context)
        {
            if (context.User?.Claims != null && context.User.Claims.Any())
            {
                Console.WriteLine("===== User Claims =====");
                foreach (var claim in context.User.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("No claims found in HttpContext.");
            }
            return int.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out var userId)
                ? userId : 0;
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Base.Utils
{
    public static class EzUtils
    {
        private static readonly TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public static string DecodeVerifyEmailToken(string token, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = configuration["JwtSettings:Secret"]!;
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Subject;
            }
            catch
            {
                return null;
            }
        }


        public static string GenerateOrderCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string last9Digits = timestamp.ToString().Substring(4, 9);

            return $"TXN{last9Digits}";
        }

        public static string GenerateCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string last9Digits = timestamp.ToString().Substring(4, 9);

            return last9Digits;
        }

        public static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            TimeZoneInfo vietNamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietNamTimeZone);
        }


        public static DateTime GetCurrentVNTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime vnTime = ConvertToVietnamTime(utcNow);
            return vnTime;
        }

        public static IFormFile ConvertToIFormFile(string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var stream = new MemoryStream(fileBytes);

            IFormFile file = new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return file;
        }


        public static DateTime GetCurrentDateTime()
        {
            DateTime utcNow = DateTime.UtcNow.AddHours(7);
            return utcNow;
        }

        public static string GetCurrentDateTimeAsString(string format = "yyyy-MM-dd HH:mm")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }

        public static string GetCurrentDateAsString(string format = "yyyy-MM-dd")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }


        public static string GenerateVerifyEmailToken(IConfiguration _configuration, string email)
        {
            var claim = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (_configuration == null)
            {
                Console.WriteLine("Configuration is null");
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public static string GenerateRefreshToken(int length = 64)
        {
            var randomNumber = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}

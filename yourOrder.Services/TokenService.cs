using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.Identity;
using yourOrder.Core.Services;

namespace yourOrder.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string CreateToken(AppUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName ,user.DisplayName)
            };

            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role , role)));

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); // just saving


            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256); // hashing algorithm 

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"], // header
                expires: DateTime.Now.AddDays(1),

                claims: authClaims, // payload

                signingCredentials: creds // signature
            );
            return new JwtSecurityTokenHandler().WriteToken(token); // xxxxx.yyyyy.zzzzz

        }
        public RefreshToken GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var randomString =  Convert.ToBase64String(randomBytes);
            return new RefreshToken
            {
                Token = randomString,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow,
                
            };
        }        
    }
}

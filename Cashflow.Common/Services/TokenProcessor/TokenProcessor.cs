using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cashflow.Core;
using Cashflow.Core.Configurations;
using Microsoft.IdentityModel.Tokens;

namespace Cashflow.Common.Services.TokenProcessor
{
    public class TokenProcessor : ITokenProcessor
    {
        private readonly TokenConfiguration _tokenConfiguration;

        public TokenProcessor(TokenConfiguration tokenConfiguration)
        {
            _tokenConfiguration = tokenConfiguration;
        }

        public string GenerateToken(Claim[] claims)
        {
            if (claims is null || claims.Length == 0)
                throw new ArgumentException("Arguments to create token are not valid.");

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _tokenConfiguration.Issuer,
                Audience = _tokenConfiguration.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.TokenExpiresInMinutes),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return token;
        }

        public bool IsValidToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken _);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken _);

            return tokenValid.Claims;
        }

        #region Helpers
        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(_tokenConfiguration.SecurityString);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _tokenConfiguration.Issuer,
                ValidateAudience = true,
                ValidAudience = _tokenConfiguration.Audience,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
        #endregion
    }
}

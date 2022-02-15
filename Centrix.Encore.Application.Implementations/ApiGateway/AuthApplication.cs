using Centrix.Encore.Application.Interfaces;
using Centrix.Encore.Application.Interfaces.ApiGateway;
using Centrix.Encore.Common;
using Centrix.Encore.Common.Exceptions;
using Centrix.Encore.Dto.Base;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Application.Implementations.ApiGateway
{
    public class AuthApplication : IAuthApplication
    {
        private readonly AppSetting _settings;
        public AuthApplication(IOptions<AppSetting> settings)
        {
            _settings = settings.Value;
        }
        public async Task<ResponseDTO> AuthUser(string userName, string password)
        {
            var response = new ResponseDTO();
            return await BuildUserInformationToken(userName, "Cliente", "Sucursal", "CodigoSistema");
        }
        private async Task<ResponseDTO> BuildUserInformationToken(string userName, string codigoCliente, string codigoSucursal, string codigoSistema, bool generateToken = true)
        {
            var response = new ResponseDTO();
            var jwtConfig = _settings.JWTConfigurations;
            var token = GenerateToken(userName, codigoCliente, codigoSucursal, codigoSistema, jwtConfig);
            response.Data = new
            {
                Expiration = DateTime.UtcNow.AddHours(jwtConfig.ExpirationTimeHours),
                Token = token
            };
            return response;
        }
        private string GenerateToken(string userName, string codigoCliente, string codigoSucursal, string codigoSistema, JWTConfiguration jWTConfiguration)
        {
            DateTime expires = DateTime.UtcNow.AddHours(jWTConfiguration.ExpirationTimeHours);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jWTConfiguration.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(Constants.Core.UserClaims.UserName,userName),
                    new Claim(Constants.Core.UserClaims.CodigoCliente,codigoCliente),
                    new Claim(Constants.Core.UserClaims.CodigoSucursal,codigoSucursal),
                    new Claim(Constants.Core.UserClaims.CodigoSistema,codigoSistema)
                }),

                Expires = expires,
                Audience = jWTConfiguration.Aud,
                Issuer = jWTConfiguration.Iss,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

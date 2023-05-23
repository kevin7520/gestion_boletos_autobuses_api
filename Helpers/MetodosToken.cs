using gestion_boletos_autobuses_api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace gestion_boletos_autobuses_api.Helpers
{
    public class MetodosToken
    {
        private readonly IConfiguration _configuration;
        public string CrearToken(Credenciales credenciales)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "idUser"),
                new Claim(ClaimTypes.Name, credenciales.correo),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.
                                        GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}

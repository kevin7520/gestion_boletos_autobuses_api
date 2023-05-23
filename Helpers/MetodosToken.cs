using gestion_boletos_autobuses_api.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
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
        public static int validarToken(HttpContext httpContext)
        {
            try
            {
                string Jwt = httpContext.Request.Headers[HeaderNames.Authorization];
                Jwt = Jwt?.Replace("bearer", null)?.Trim();
                if (string.IsNullOrEmpty(Jwt)) throw new Exception("Token no recibido");
                var tokenSecure = new JwtSecurityTokenHandler().ReadToken(Jwt) as JwtSecurityToken;
                int user = Convert.ToInt32(tokenSecure.Payload["nameid"]);
                return user; // Información adicional
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using RESTAPI_CORE.Modelos.DTO;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using RESTAPI_CORE.Modelos;

namespace RESTAPI_CORE.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretKey;
        private readonly string cadenaSQL;

        public AutenticacionController(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        // LISTAR
        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] UsuarioDTO request)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_autenticar_usuario", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("correo", request.correo);
                    cmd.Parameters.AddWithValue("Clave", request.clave);
                    cmd.ExecuteNonQuery();


                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            if (rd["respuesta"].ToString() == "Inicio con exito")
                            {

                                var keyBytes = Encoding.ASCII.GetBytes(secretKey);  // primero se crea la llave secreta de tipo byte para poder encriptar el token
                                var claims = new ClaimsIdentity(); // se crea un objeto de tipo ClaimsIdentity para poder agregarle los claims al token que se va a crear (claims: son los datos que se van a guardar en el token)
                                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo)); // se agrega el claim al objeto claims (el claim es el dato que se va a guardar en el token)
                                var tokenDescriptor = new SecurityTokenDescriptor // se crea un objeto de tipo SecurityTokenDescriptor para poder crear el token 
                                {
                                    Subject = claims, // se agrega el objeto claims al objeto tokenDescriptor para que se guarde en el token
                                    Expires = DateTime.UtcNow.AddMinutes(5), // se agrega la fecha de expiracion del token (en este caso 5 minutos)
                                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature) // se agrega la llave secreta para encriptar el token (en este caso se usa el algoritmo HmacSha256Signature)
                                }; // se crea el token con los datos que se agregaron al objeto tokenDescriptor

                                var tokenHandler = new JwtSecurityTokenHandler(); // se crea un objeto de tipo JwtSecurityTokenHandler para poder crear el token con los datos que se agregaron al objeto tokenDescriptor 
                                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor); // se crea el token con los datos que se agregaron al objeto tokenDescriptor
                                string tokencreado = tokenHandler.WriteToken(tokenConfig);  // se crea una variable tokencreado que guarda el token creado en formato string

                                return Ok(new { token = tokencreado, usuario = request });
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest("Usuario o contraseña incorrectos");
        }


        //[HttpPost]

        //public IActionResult Validar([FromBody] Usuario request)
        //{
        //    if (request.correo == "adsi2022@sena.com" && request.clave == "1234")
        //    {

        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
        //    }
        //}
    }
}

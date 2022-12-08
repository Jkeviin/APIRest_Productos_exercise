using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTAPI_CORE.Modelos;
using RESTAPI_CORE.Modelos.DTO;
using System.Data;
using System.Data.SqlClient;

namespace RESTAPI_CORE.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly string cadenaSQL;
        public UsuarioController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        // LISTAR
        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_usuarios", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Usuario
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                correo = rd["Correo"].ToString(),
                                clave = rd["Clave"].ToString()
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });
            }
        }

        // OBTENER
        [HttpGet]
        [Route("Obtener/{Id:int}")]
        public IActionResult Obtener(int IdUsuario)
        {
            List<Usuario> lista = new List<Usuario>();
            Usuario ousuario = new Usuario();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_lista_usuarios", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Usuario
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                correo = rd["Correo"].ToString(),
                                clave = rd["Clave"].ToString()
                            });
                        }
                    }
                }
                ousuario = lista.Where(item => item.Id == IdUsuario).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = ousuario });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = ousuario });
            }
        }

        // GUARDAR
        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] UsuarioDTO objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_usuario", conexion);
                    cmd.Parameters.AddWithValue("Correo", objeto.correo);
                    cmd.Parameters.AddWithValue("Clave", objeto.clave);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Agregado", usuario = objeto });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        // EDITAR
        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] Usuario objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_editar_usuario", conexion);
                    cmd.Parameters.AddWithValue("Id", objeto.Id == 0 ? DBNull.Value : objeto.Id);
                    cmd.Parameters.AddWithValue("Correo", objeto.correo is null ? DBNull.Value : objeto.correo);
                    cmd.Parameters.AddWithValue("Clave", objeto.clave is null ? DBNull.Value : objeto.clave);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado con éxito", usuario_editado = objeto.Id });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        // ELIMINAR
        [HttpDelete]
        [Route("Eliminar/{Id:int}")]
        public IActionResult Eliminar(int IdUsuario)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_eliminar_usuario", conexion);
                    cmd.Parameters.AddWithValue("Id", IdUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Eliminado el usuario " + IdUsuario });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}

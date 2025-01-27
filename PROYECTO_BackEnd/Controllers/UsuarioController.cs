using CLASES;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PROYECTO_BackEnd.Shared;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;

namespace PROYECTO_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioController (ILogger<UsuarioController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CrearUsuario(Usuario usuarios)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(usuarios);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetUsuario,
                cadenaConexion,
                usuarios.transaccion,
                xmlParam.ToString()
            );
            string respuesta = null;
            string leyenda = null;
            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    DataRow firstRow = dsResult.Tables[0].Rows[0];
                    respuesta = firstRow["respuesta"].ToString();
                    leyenda = firstRow["leyenda"].ToString();

                }
                catch (Exception ex)
                {
                    _logger.LogError("Error al procesar los datos: " + ex.ToString());
                    return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
                }
            }
            else
            {
                _logger.LogWarning("No se encontraron registros en la consulta.");
                return NotFound(new { respuesta = "ERROR", leyenda = "No se encontraron registros." });
            }

            return Ok(new
            {
                respuesta = respuesta,
                leyenda = leyenda,
            });
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<Usuario>>> recuperarUsuario([FromBody] Usuario usuario)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(usuario);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetUsuario,
                cadenaConexion,
                usuario.transaccion,
                xmlParam.ToString()
            );

            List<Usuario> usuarios = new List<Usuario>();
            string respuesta = null;
            string leyenda = null;
            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    DataRow firstRow = dsResult.Tables[0].Rows[0];
                    respuesta = "exito";
                    leyenda = "Usuario obtenido";
                    foreach (DataRow row in dsResult.Tables[0].Rows)
                    {
                        Usuario response = new Usuario()
                        {
                            usuario = row["usuario"].ToString(),
                            clave = row["Clave"].ToString()
                        };
                        usuarios.Add(response);
          
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error al procesar los datos: " + ex.ToString());
                }
            }
            else
            {
                _logger.LogWarning("No se encontraron registros en la consulta.");
                return Ok(new
                {
                    respuesta = "error",
                    leyenda = "error en las credenciales",
                });

            }

            return Ok(new
            {
                respuesta = respuesta,
                leyenda = leyenda,
                Usuario = usuarios,

            });
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<Usuario>>> getUsuarioByOpcion([FromBody]Usuario usuario)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(usuario);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetUsuario,
                cadenaConexion,
                usuario.transaccion,
                xmlParam.ToString()
            );

            List<Usuario> usuarios = new List<Usuario>();
            string respuesta = null;
            string leyenda = null;
            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    DataRow firstRow = dsResult.Tables[0].Rows[0];
                    respuesta = "exito";
                    leyenda = "Usuario obtenido";
                    foreach (DataRow row in dsResult.Tables[0].Rows)
                    {
                        Usuario response = new Usuario()
                        {
                            usuario = row["Usuario"].ToString(),
                            clave = row["Clave"].ToString()
                        };
                        usuarios.Add(response);
                    return Ok(new
                    {
                        respuesta = respuesta,
                        leyenda = leyenda,
                        token = JsonConvert.SerializeObject(CrearToken(response))
                    });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error al procesar los datos: " + ex.ToString());
                }
            }
            else
            {
                _logger.LogWarning("No se encontraron registros en la consulta.");
                return Ok(new
                {
                    respuesta = "error",
                    leyenda = "error en las credenciales",
                    menssage = JsonConvert.SerializeObject("Error credenciales")
                });

            }

            return Ok(new
            {
                respuesta = respuesta,
                leyenda = leyenda,
                Usuario = usuario,
                token = JsonConvert.SerializeObject(CrearToken(usuario))

            });
        }

        private string CrearToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,usuario.id.ToString()),
                new Claim(ClaimTypes.Name,usuario.usuario)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var credenciales = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credenciales,
                Expires = DateTime.Now.AddDays(1),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

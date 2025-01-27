using CLASES;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Xml.Linq;

namespace PROYECTO_BackEnd.Controllers
{
    public class ClienteController : ControllerBase
    {

        private readonly ILogger<UsuarioController> _logger;
        private readonly IConfiguration _configuration;

        public ClienteController(ILogger<UsuarioController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Route("EliminarCliente/{id_cliente}")]
        [HttpDelete] // issac dijo que use DELETE !!!!!!!!!!!!!!!!!!!!!!!!!
        public async Task<ActionResult> EliminarCliente(int id_cliente)
        {
            // Configurar la cadena de conexión
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            // Crear el objeto cliente para la eliminación
            Cliente cliente = new Cliente
            {
                id_cliente = id_cliente,
                transaccion = "ELIMINAR_CLIENTE"
            };

            // Construcción del XML que se va a enviar al stored procedure
            XDocument xmlParam = Shared.DBXmlMethods.GetXml(cliente);

            // Ejecución del stored procedure
            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetCliente,
                cadenaConexion,
                cliente.transaccion,
                xmlParam.ToString()
            );

            // Verificación de los resultados
            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    DataRow firstRow = dsResult.Tables[0].Rows[0];
                    string respuesta = firstRow["respuesta"].ToString();
                    string leyenda = firstRow["leyenda"].ToString();

                    // Devolver la respuesta al cliente
                    if (respuesta == "OK")
                    {
                        return Ok(new { respuesta, leyenda });
                    }
                    else
                    {
                        return BadRequest(new { respuesta, leyenda });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error al procesar la eliminación del cliente: " + ex.ToString());
                    return StatusCode(500, new { respuesta = "ERROR", leyenda = "Error en el servidor" });
                }
            }
            else
            {
                _logger.LogWarning("No se encontraron registros en la consulta de eliminación.");
                return NotFound(new { respuesta = "ERROR", leyenda = "Cliente no encontrado" });
            }
        }



        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<Cliente>>> getCliente([FromBody] Cliente cliente)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(cliente);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetCliente,
                cadenaConexion,
                cliente.transaccion,
                xmlParam.ToString()
            );

            List<Cliente> clientes = new List<Cliente>();
            string respuesta = null;
            string leyenda = null;
            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    DataRow firstRow = dsResult.Tables[0].Rows[0];

                

                    foreach (DataRow row in dsResult.Tables[0].Rows)
                    {
                        Cliente response = new Cliente()
                        {
                            id_cliente = Convert.ToInt32(row["ID"]),
                            nombre = row["Nombre"].ToString(),
                            apellido = row["Apellido"].ToString(),
                            edad = Convert.ToInt32(row["Edad"]),
                            email = row["Email"].ToString(),
                            telefono = row["Telefono"].ToString(),
                            servicioPrestado = row["ServicioPrestado"].ToString(),
                            fechaRegistro = Convert.ToDateTime(row["FechaRegistro"]),
                            fechaActualizacion = row["FechaActualizacion"] != DBNull.Value
                                ? (DateTime?)Convert.ToDateTime(row["FechaActualizacion"])
                                : null,
                        };
                        clientes.Add(response);
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
                Cliente = clientes,

            });
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<Cliente>>> crearCliente([FromBody] Cliente cliente)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(cliente);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetCliente,
                cadenaConexion,
                cliente.transaccion,
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

            });
        }
    }
}

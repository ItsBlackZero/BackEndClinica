using CLASES;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Data;
using System.Xml.Linq;

namespace PROYECTO_BackEnd.Controllers
{
    public class DoctorController: ControllerBase
    {
        private readonly ILogger<DoctorController> _logger;
        private readonly IConfiguration _configuration;

        public DoctorController(ILogger<DoctorController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [Route("EliminarDoctor/{id_doctor}")]
        [HttpDelete] // issac dijo que use DELETE !!!!!!!!!!!!!!!!!!!!!!!!!
        public async Task<ActionResult> EliminarCliente(int id_doctor)
        {
            // Configurar la cadena de conexión
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            // Crear el objeto cliente para la eliminación
            Doctor doctor= new  Doctor
            {
                id = id_doctor,
                transaccion = "ELIMINAR_DOCTOR"
            };

            // Construcción del XML que se va a enviar al stored procedure
            XDocument xmlParam = Shared.DBXmlMethods.GetXml(doctor);

            // Ejecución del stored procedure
            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetDoctor,
                cadenaConexion,
                doctor.transaccion,
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
        public async Task<ActionResult<List<Cliente>>> crearDoctor([FromBody] Doctor doctor)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(doctor);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetDoctor,
                cadenaConexion,
                doctor.transaccion,
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

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult<List<Doctor>>> getDoctor([FromBody] Doctor doctor)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(doctor);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetDoctor,
                cadenaConexion,
                doctor.transaccion,
                xmlParam.ToString()
            );

            List<Doctor> doctores= new List<Doctor>();
            string respuesta = null;
            string leyenda = null;

            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    respuesta = "Exito";
                    leyenda = "Doctores Obtenidas";
                    foreach (DataRow row in dsResult.Tables[0].Rows)
                    {
                        Doctor response = new Doctor()
                        {
                            id = Convert.ToInt32(row["Id"]),
                            nombre = row["Nombre"].ToString(),
                            apellido = row["Apellido"].ToString(),
                            telefono = row["Telefono"].ToString(),
                            especialidad = row["Especialidad"].ToString(),
                            email = row["Email"].ToString(),
                            fechaRegistro = Convert.ToDateTime(row["FechaRegistro"]),
                            estado = row["Estado"].ToString()
                        };

                        doctores.Add(response);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error al procesar los datos: " + ex.ToString());
                    return StatusCode(500, new { respuesta = "error", leyenda = "Error al procesar los datos" });
                }
            }
            else
            {
                _logger.LogWarning("No se encontraron registros en la consulta.");
                return Ok(new
                {
                    respuesta = "error",
                    leyenda = "No se encontraron citas para el cliente",
                });
            }

            return Ok(new
            {
                respuesta = respuesta,
                leyenda = leyenda,
                Data = doctores,
            });
        }
    }
}

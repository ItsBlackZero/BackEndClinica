﻿using CLASES;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml.Linq;

namespace PROYECTO_BackEnd.Controllers
{
    public class CitaController : ControllerBase
    {
        private readonly ILogger<CitaController> _logger;
        private readonly IConfiguration _configuration;

        public CitaController(ILogger<CitaController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [Route("EliminarCliente/{id_cita}")]
        [HttpDelete] // issac dijo que use DELETE !!!!!!!!!!!!!!!!!!!!!!!!!
        public async Task<ActionResult> EliminarCita(int id_cita)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            Cita cita = new Cita
            {
                id = id_cita,
                transaccion = "ELIMINAR_CITA"
            };

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(cita);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetCliente,
                cadenaConexion,
                cita.transaccion,
                xmlParam.ToString()
            );

            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    DataRow firstRow = dsResult.Tables[0].Rows[0];
                    string respuesta = firstRow["respuesta"].ToString();
                    string leyenda = firstRow["leyenda"].ToString();

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
        public async Task<ActionResult<List<Cita>>> crearCita([FromBody] Cita cita)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(cita);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetCita,
                cadenaConexion,
                cita.transaccion,
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
        public async Task<ActionResult<List<Cita>>> getCita([FromBody] Cita cita)
        {
            var cadenaConexion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ConnectionStrings")["CadenaSQL"];

            XDocument xmlParam = Shared.DBXmlMethods.GetXml(cita);

            DataSet dsResult = await Shared.DBXmlMethods.EjecutaBase(
                Shared.NameStoredProcedure.SP_GetCita,
                cadenaConexion,
                cita.transaccion,
                xmlParam.ToString()
            );

            List<Cita> citas = new List<Cita>();
            string respuesta = null;
            string leyenda = null;

            if (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
            {
                try
                {
                    respuesta = "Exito";
                    leyenda = "Citas Obtenidas";
                    foreach (DataRow row in dsResult.Tables[0].Rows)
                    {
                        Cita response = new Cita
                        {
                         
                            id = Convert.ToInt32(row["Id"]),
                            id_cliente = Convert.ToInt32(row["ClienteId"]),
                            
                            id_doctor= Convert.ToInt32(row["DoctorId"]),

                            cliente = row["Cliente"].ToString(),
                            doctor = row["Doctor"].ToString(),
                            fecha = Convert.ToDateTime(row["Fecha"]),
                            Hora = (row["Hora"].ToString()),
                            estado = row["Estado"].ToString(),
                            observaciones = row["Observaciones"].ToString(),
                            fechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                            fechaActualizacion = Convert.ToDateTime(row["FechaActualizacion"]),

                        };

                        citas.Add(response);
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
                Data = citas,
            });
        }
    }
}

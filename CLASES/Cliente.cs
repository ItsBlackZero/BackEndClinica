using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLASES
{
    public class Cliente
    {
        public int? id_cliente {  get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public int? edad {  get; set; }
        public string? email {  get; set; }
        public string? telefono { get; set; }
        public string? servicioPrestado { get; set; }
        public DateTime? fechaRegistro { get; set; }
        public DateTime? fechaActualizacion { get; set; }


        public string? transaccion { get; set; }
    }
}

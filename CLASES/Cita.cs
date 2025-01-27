using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLASES
{
    public class Cita
    {
        public int id { get; set; }

        public int id_cliente { get; set; }
        public int id_doctor { get; set; }

        public string  cliente { get; set; }
        public string doctor { get; set; }
        public DateTime? fecha { get; set; }
        public string Hora { get; set; }

        public EstadoCita? estado { get; set; }
        public string? observaciones { get; set; }
        public DateTime? fechaCreacion   { get; set; }
        public DateTime? fechaActualizacion { get; set; }

        public string? transaccion { get; set; }


    }
}

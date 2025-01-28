using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLASES
{
    public class Doctor
    {
        public int id {  get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string especialidad  { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string estado { get; set; }

        public string transaccion {  get; set; }
    }
}

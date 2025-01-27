using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLASES
{
    public class Usuario
    {
        public int? id {  get; set; }   
        public string? usuario {  get; set; }
        public string? clave { get; set; }

        public string? email { get; set; }

        public string? transaccion { get; set; }
    }
}

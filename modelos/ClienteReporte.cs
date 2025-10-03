using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO1.modelos
{
    public class ClienteReporte
    {
        public string Nombre { get; set; }
        public int Compras { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
    }
}

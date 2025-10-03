using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO1.modelos
{
    public class VendedorReporte
    {
        public string Nombre { get; set; }
        public int Ventas { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
    }
}

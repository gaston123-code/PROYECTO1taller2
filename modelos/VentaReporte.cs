using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO1.modelos
{
    public class VentaReporte
    {
        public int IdVenta { get; set; }
        public string Cliente { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
    }
}


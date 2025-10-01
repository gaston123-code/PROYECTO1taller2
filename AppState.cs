using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ProyectoVentas/Core/AppState.cs
using System.Collections.Generic;
using System.ComponentModel;

namespace PROYECTO1
{
    public static class AppState
    {
        public static BindingList<ClienteVM> Clientes { get; } = new();
        public static BindingList<ProductoVM> Productos { get; } = new();
        public static BindingList<Venta> Ventas { get; } = new();
    }

    // Modelos compartidos (reutilizá lo que ya tenés)
    public class ClienteVM
    {
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Sexo { get; set; }
        public System.DateTime? Nacimiento { get; set; }
        public string Telefono { get; set; }
    }

    public class ProductoVM
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Stock { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCosto { get; set; }
        public int Talle { get; set; }
        public string ImagenPath { get; set; } // opcional, si querés ruta
    }

    public class Venta
    {
        public System.DateTime Fecha { get; set; }
        public string Metodo { get; set; }
        public string DniCliente { get; set; }
        public string NombreCliente { get; set; }
        public List<VentaItem> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class VentaItem
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int Talle { get; set; }
        public int Cantidad { get; set; }
        public decimal SubTotal { get; set; }
        public string Imagen { get; set; } // texto en tu grilla
        public System.DateTime Fecha { get; set; }
        public string Metodo { get; set; }
    }
}

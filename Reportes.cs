using PROYECTO1;
using PROYECTO1.modelos;
using System;
using System.Windows.Forms;

namespace ProyectoReportes
{
    public partial class Reportes : Form
    {
        public Reportes()
        {
            InitializeComponent();

            // Enganchar eventos
            buttonVentas.Click += BtnVerVentas_Click;
            buttonProductos.Click += BtnVerProductos_Click;
            buttonVendedores.Click += BtnVerVendedores_Click;
            buttonClientes.Click += BtnVerClientes_Click;
            buttonMetodos.Click += BtnVerMetodosPago_Click;
        }

        private void Reportes_Load(object sender, EventArgs e)
        {
            // Inicializar filtros
            FechaDesde.Value = new DateTime(DateTime.Today.Year, 1, 1);
            FechaHasta.Value = DateTime.Today;

            comboBoxOrden.Items.AddRange(new string[] { "Ascendente", "Descendente" });
            comboBoxOrden.SelectedIndex = 0;

        }

        // Lista de ventas simuladas
        private List<VentaReporte> _ventas = new List<VentaReporte>
{
    new VentaReporte { IdVenta = 1, Cliente = "Juan Pérez", Total = 2500, Fecha = new DateTime(2025, 1, 15) },
    new VentaReporte { IdVenta = 2, Cliente = "Ana Gómez", Total = 4800, Fecha = new DateTime(2025, 3, 10) },
    new VentaReporte { IdVenta = 3, Cliente = "Carlos Ruiz", Total = 1200, Fecha = new DateTime(2025, 5, 5) }
};

        private List<ProductoReporte> _productos = new List<ProductoReporte>
{
    new ProductoReporte { Nombre = "Camiseta", Stock = 50, Precio = 2500, Fecha = new DateTime(2025, 1, 20) },
    new ProductoReporte { Nombre = "Pantalón", Stock = 30, Precio = 4800, Fecha = new DateTime(2025, 3, 15) },
    new ProductoReporte { Nombre = "Zapatillas", Stock = 15, Precio = 12000, Fecha = new DateTime(2025, 5, 8) }
};

        private List<ClienteReporte> _clientes = new List<ClienteReporte>
{
    new ClienteReporte { Nombre = "Juan Pérez", Compras = 5, Total = 12000, Fecha = new DateTime(2025, 2, 10) },
    new ClienteReporte { Nombre = "Ana Gómez", Compras = 3, Total = 7500, Fecha = new DateTime(2025, 4, 12) }
};

        private List<VendedorReporte> _vendedores = new List<VendedorReporte>
{
    new VendedorReporte { Nombre = "Pedro López", Ventas = 15, Total = 50000, Fecha = new DateTime(2025, 2, 20) },
    new VendedorReporte { Nombre = "María Torres", Ventas = 20, Total = 75000, Fecha = new DateTime(2025, 4, 18) }
};

        private List<MetodoPagoReporte> _metodosPago = new List<MetodoPagoReporte>
{
    new MetodoPagoReporte { Nombre = "Efectivo", Porcentaje = 40, Fecha = new DateTime(2025, 1, 25) },
    new MetodoPagoReporte { Nombre = "Tarjeta Crédito", Porcentaje = 35, Fecha = new DateTime(2025, 3, 5) },
    new MetodoPagoReporte { Nombre = "Transferencia", Porcentaje = 25, Fecha = new DateTime(2025, 5, 12) }
};

        private void BtnVerVentas_Click(object sender, EventArgs e)
        {
            IEnumerable<VentaReporte> filtradas = _ventas;

            // 🔹 Filtro opcional por fechas
            if (guna2CheckBox1.Checked)
            {
                DateTime desde = FechaDesde.Value.Date;
                DateTime hasta = FechaHasta.Value.Date;
                filtradas = filtradas.Where(v => v.Fecha >= desde && v.Fecha <= hasta);
            }

            // 🔹 Ordenamiento según comboBoxOrden
            string orden = comboBoxOrden.SelectedItem?.ToString();
            if (orden == "Mayor a menor")
                filtradas = filtradas.OrderByDescending(v => v.Total);
            else if (orden == "Menor a mayor")
                filtradas = filtradas.OrderBy(v => v.Total);
            // Si es "Todos", no se aplica orden

            if (!filtradas.Any())
            {
                MessageBox.Show("No hay ventas en el rango de fechas seleccionado.");
                return;
            }

            string contenido = "📊 REPORTE DE VENTAS\n" +
                               "-----------------------------\n";

            foreach (var v in filtradas)
            {
                contenido += $"Venta #{v.IdVenta} - Cliente: {v.Cliente} - Total: ${v.Total} - Fecha: {v.Fecha:dd/MM/yyyy}\n";
            }

            new ReportesVista("Reporte de Ventas", contenido).ShowDialog();
        }

        private void BtnVerProductos_Click(object sender, EventArgs e)
        {
            IEnumerable<ProductoReporte> filtrados = _productos;

            // 🔹 Filtro opcional por fechas
            if (guna2CheckBox1.Checked)
            {
                DateTime desde = FechaDesde.Value.Date;
                DateTime hasta = FechaHasta.Value.Date;
                filtrados = filtrados.Where(p => p.Fecha >= desde && p.Fecha <= hasta);
            }

            // 🔹 Ordenamiento según comboBoxOrden (por Precio)
            string orden = comboBoxOrden.SelectedItem?.ToString();
            if (orden == "Mayor a menor")
                filtrados = filtrados.OrderByDescending(p => p.Precio);
            else if (orden == "Menor a mayor")
                filtrados = filtrados.OrderBy(p => p.Precio);

            if (!filtrados.Any())
            {
                MessageBox.Show("No hay productos en el rango de fechas seleccionado.");
                return;
            }

            string contenido = "📦 REPORTE DE PRODUCTOS\n" +
                               "-----------------------------\n";

            foreach (var p in filtrados)
            {
                contenido += $"{p.Nombre} - Stock: {p.Stock} - Precio: ${p.Precio} - Fecha: {p.Fecha:dd/MM/yyyy}\n";
            }

            new ReportesVista("Reporte de Productos", contenido).ShowDialog();
        }

        private void BtnVerVendedores_Click(object sender, EventArgs e)
        {
            IEnumerable<VendedorReporte> filtrados = _vendedores;

            if (guna2CheckBox1.Checked)
            {
                DateTime desde = FechaDesde.Value.Date;
                DateTime hasta = FechaHasta.Value.Date;
                filtrados = filtrados.Where(v => v.Fecha >= desde && v.Fecha <= hasta);
            }

            string orden = comboBoxOrden.SelectedItem?.ToString();
            if (orden == "Mayor a menor")
                filtrados = filtrados.OrderByDescending(v => v.Total);
            else if (orden == "Menor a mayor")
                filtrados = filtrados.OrderBy(v => v.Total);

            if (!filtrados.Any())
            {
                MessageBox.Show("No hay vendedores en el rango de fechas seleccionado.");
                return;
            }

            string contenido = "🧑‍💼 REPORTE DE VENDEDORES\n" +
                               "-----------------------------\n";

            foreach (var v in filtrados)
            {
                contenido += $"{v.Nombre} - Ventas: {v.Ventas} - Total: ${v.Total} - Fecha: {v.Fecha:dd/MM/yyyy}\n";
            }

            new ReportesVista("Reporte de Vendedores", contenido).ShowDialog();
        }

        private void BtnVerClientes_Click(object sender, EventArgs e)
        {
            IEnumerable<ClienteReporte> filtrados = _clientes;

            // 🔹 Filtro opcional por fechas
            if (guna2CheckBox1.Checked)
            {
                DateTime desde = FechaDesde.Value.Date;
                DateTime hasta = FechaHasta.Value.Date;
                filtrados = filtrados.Where(c => c.Fecha >= desde && c.Fecha <= hasta);
            }

            // 🔹 Ordenamiento según comboBoxOrden (por Total gastado)
            string orden = comboBoxOrden.SelectedItem?.ToString();
            if (orden == "Mayor a menor")
                filtrados = filtrados.OrderByDescending(c => c.Total);
            else if (orden == "Menor a mayor")
                filtrados = filtrados.OrderBy(c => c.Total);

            if (!filtrados.Any())
            {
                MessageBox.Show("No hay clientes en el rango de fechas seleccionado.");
                return;
            }

            string contenido = "👥 REPORTE DE CLIENTES\n" +
                               "-----------------------------\n";

            foreach (var c in filtrados)
            {
                contenido += $"{c.Nombre} - Compras: {c.Compras} - Total: ${c.Total} - Fecha: {c.Fecha:dd/MM/yyyy}\n";
            }

            new ReportesVista("Reporte de Clientes", contenido).ShowDialog();
        }


        private void BtnVerMetodosPago_Click(object sender, EventArgs e)
        {
            IEnumerable<MetodoPagoReporte> filtrados = _metodosPago;

            if (guna2CheckBox1.Checked)
            {
                DateTime desde = FechaDesde.Value.Date;
                DateTime hasta = FechaHasta.Value.Date;
                filtrados = filtrados.Where(m => m.Fecha >= desde && m.Fecha <= hasta);
            }

            string orden = comboBoxOrden.SelectedItem?.ToString();
            if (orden == "Mayor a menor")
                filtrados = filtrados.OrderByDescending(m => m.Porcentaje);
            else if (orden == "Menor a mayor")
                filtrados = filtrados.OrderBy(m => m.Porcentaje);

            if (!filtrados.Any())
            {
                MessageBox.Show("No hay métodos de pago en el rango de fechas seleccionado.");
                return;
            }

            string contenido = "💳 REPORTE DE MÉTODOS DE PAGO\n" +
                               "-----------------------------\n";

            foreach (var m in filtrados)
            {
                contenido += $"{m.Nombre} - {m.Porcentaje}% de las ventas - Fecha: {m.Fecha:dd/MM/yyyy}\n";
            }

            new ReportesVista("Reporte de Métodos de Pago", contenido).ShowDialog();
        }
    }
}
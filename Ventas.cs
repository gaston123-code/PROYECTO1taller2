using formulario_cliente;
using PROYECTO1;
using PROYECTO1.modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Guna.UI2.WinForms.Suite.Descriptions;

namespace ProyectoVentas
{
    public partial class Ventas : Form
    {
        // Producto en la venta
        public class ProductoVenta
        {
            public int IdProducto { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Categoria { get; set; }
            public string Talla { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioVenta { get; set; }
            public decimal SubTotal => Cantidad * PrecioVenta;

            // Nueva propiedad para la imagen
            public string ImagenRuta { get; set; } // guardamos la ruta del archivo

        }

        // Cliente
        private class ClienteVenta
        {
            public int IdCliente { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string DNI { get; set; }
            public string Correo { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string Genero { get; set; }
            public DateTime FechaNacimiento { get; set; }
        }

        // Venta
        private class Venta
        {
            public int IdVenta { get; set; }
            public string NombreCliente { get; set; }
            public string DNICliente { get; set; }
            public decimal PrecioTotal { get; set; }
            public DateTime FechaVenta { get; set; }
            public string UsuarioVendedor { get; set; }
        }

        private readonly List<ProductoVenta> _productosEnVenta = new List<ProductoVenta>();
        private readonly List<Venta> _ventas = new List<Venta>();
        private ClienteVenta _clienteSeleccionado = null;

        private int _contadorVentas = 1;
        private string _usuarioVendedor = "Admin"; // Simulación de usuario logueado

        public Ventas()
        {
            InitializeComponent();

            buttonCliente.Click += BtnBuscarCliente_Click;
            buttonBuscar.Click += BtnBuscarProducto_Click;
            buttonAgregar.Click += BtnAgregarProducto_Click;
            buttonGuardar.Click += BtnGuardar_Click;
            buttonEliminar.Click += BtnEliminar_Click;

            this.Load += Ventas_Load;
        }

        private void Ventas_Load(object sender, EventArgs e)
        {
            if (SesionActual.Usuario != null)
            {
                labelNombreApellidoVendedor.Text =
                    $"{SesionActual.Usuario.Nombre} {SesionActual.Usuario.Apellido}";
            }

            ConfigurarGrillaVentas();
            MostrarVentas();
            fecha.Text = DateTime.Today.ToShortDateString();

            // Inicializar métodos de pago si está vacío
            if (comboBoxMetodo.Items.Count == 0)
            {
                comboBoxMetodo.Items.AddRange(new[] { "Efectivo", "Tarjeta débito", "Tarjeta crédito", "Transferencia" });
            }
        }

        private void ConfigurarGrillaVentas()
        {
            dataGridViewVentas.AutoGenerateColumns = false;
            dataGridViewVentas.Columns.Clear();

            dataGridViewVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID Venta", DataPropertyName = "IdVenta" });
            dataGridViewVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", DataPropertyName = "NombreCliente" });
            dataGridViewVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DNI Cliente", DataPropertyName = "DNICliente" });
            dataGridViewVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha Venta", DataPropertyName = "FechaVenta" });
            dataGridViewVentas.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vendedor", DataPropertyName = "UsuarioVendedor" });
        }

        private void ConfigurarGrillaDetalle()
        {
            dataGridViewDetalle.AutoGenerateColumns = false;
            dataGridViewDetalle.Columns.Clear();

            // 🔹 Definición de columnas con Name, HeaderText y DataPropertyName
            dataGridViewDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Producto",
                HeaderText = "Producto",
                DataPropertyName = "Nombre"
            });

            dataGridViewDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Precio",
                HeaderText = "Precio",
                DataPropertyName = "PrecioVenta"
            });

            dataGridViewDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                HeaderText = "Cantidad",
                DataPropertyName = "Cantidad"
            });

            dataGridViewDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubTotal",
                HeaderText = "Subtotal",
                DataPropertyName = "SubTotal"
            });

            // 🔹 Solo Cantidad editable
            dataGridViewDetalle.Columns["Producto"].ReadOnly = true;
            dataGridViewDetalle.Columns["Precio"].ReadOnly = true;
            dataGridViewDetalle.Columns["SubTotal"].ReadOnly = true;

            // 🔹 Evento para detectar cambios en cantidad
            dataGridViewDetalle.CellEndEdit += dataGridViewDetalle_CellEndEdit;
        }

        private void dataGridViewDetalle_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _productosEnVenta.Count &&
                dataGridViewDetalle.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                var producto = _productosEnVenta[e.RowIndex];

                if (int.TryParse(dataGridViewDetalle.Rows[e.RowIndex].Cells["Cantidad"].Value?.ToString(), out int nuevaCantidad))
                {
                    producto.Cantidad = nuevaCantidad;
                    dataGridViewDetalle.Refresh();
                    ActualizarTotal();
                }
                else
                {
                    MessageBox.Show("Ingrese un número válido en la cantidad.");
                    dataGridViewDetalle.Rows[e.RowIndex].Cells["Cantidad"].Value = producto.Cantidad;
                }
            }
        }

        private void MostrarVentas()
        {
            dataGridViewVentas.DataSource = null;
            dataGridViewVentas.DataSource = _ventas;
        }

        private void BtnBuscarCliente_Click(object sender, EventArgs e)
        {
            string dniIngresado = textBoxCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(dniIngresado))
            {
                MessageBox.Show("Ingrese un DNI para buscar.");
                return;
            }

            // Simulación de base de datos en memoria
            var clientesPredefinidos = new List<ClienteVenta>
    {
        new ClienteVenta
        {
            IdCliente = 1,
            Nombre = "Juan",
            Apellido = "Pérez",
            DNI = "12345678",
            Correo = "juanperez@mail.com",
            Telefono = "3794000000",
            Direccion = "Av. Siempre Viva 123",
            Genero = "Hombre",
            FechaNacimiento = new DateTime(1990, 5, 20)
        },
        new ClienteVenta
        {
            IdCliente = 2,
            Nombre = "Ana",
            Apellido = "Gómez",
            DNI = "87654321",
            Correo = "anagomez@mail.com",
            Telefono = "3794111111",
            Direccion = "Calle Falsa 456",
            Genero = "Mujer",
            FechaNacimiento = new DateTime(1992, 8, 15)
        }
    };

            // Buscar cliente por DNI
            _clienteSeleccionado = clientesPredefinidos
                .FirstOrDefault(c => c.DNI == dniIngresado);

            if (_clienteSeleccionado == null)
            {
                MessageBox.Show("Cliente no encontrado.");
                return;
            }

            // Cargar datos en los campos
            guna2TextBox5.Text = _clienteSeleccionado.Nombre;
            guna2TextBox1.Text = _clienteSeleccionado.Apellido;
            guna2TextBox2.Text = _clienteSeleccionado.DNI;
            guna2TextBox3.Text = _clienteSeleccionado.Correo;
            textBoxTelefono.Text = _clienteSeleccionado.Telefono;
            guna2TextBox4.Text = _clienteSeleccionado.Direccion;
            radioButtonHombre.Checked = _clienteSeleccionado.Genero == "Hombre";
            radioButtonMujer.Checked = _clienteSeleccionado.Genero == "Mujer";
            guna2DateTimePicker1.Value = _clienteSeleccionado.FechaNacimiento;

            

        }



        private void BtnBuscarProducto_Click(object sender, EventArgs e)
        {
            string nombreIngresado = textBoxNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreIngresado))
            {
                MessageBox.Show("Ingrese un nombre de producto para buscar.");
                return;
            }

            var productosPredefinidos = new List<dynamic>
    {
        new { Nombre = "Camiseta", Descripcion = "Tela de algodón", Categoria = "Ropa", Talle = "M", Cantidad = "1", PrecioVenta = "2500", Stock = "50" },
        new { Nombre = "Pantalón", Descripcion = "Jean azul", Categoria = "Ropa", Talle = "L", Cantidad = "1", PrecioVenta = "4800", Stock = "30" },
        new { Nombre = "Zapatillas", Descripcion = "Deportivas", Categoria = "Calzado", Talle = "42", Cantidad = "1", PrecioVenta = "12000", Stock = "15" }
    };

            var producto = productosPredefinidos
                .FirstOrDefault(p => p.Nombre.Equals(nombreIngresado, StringComparison.OrdinalIgnoreCase));

            if (producto == null)
            {
                MessageBox.Show("Producto no encontrado.");
                return;
            }

            // Cargar datos en los campos
            textBoxNombre.Text = producto.Nombre;
            textBoxDescripcion.Text = producto.Descripcion;
            labelCategoriaDato.Text = producto.Categoria;
            comboBoxTalle.Text = producto.Talle;
            textBoxCantidad.Text = producto.Cantidad;
            labelPrecioVentaDato.Text = producto.PrecioVenta;
            labelStockDato.Text = producto.Stock;

            // Diccionario de imágenes
            // 🔹 Diccionario de imágenes (puede ir aquí mismo)

            var imagenesProductos = new Dictionary<string, Image>
{
    { "Camiseta", Image.FromFile(@"C:\Users\car_a\OneDrive\Pictures\producto\camiseta.png") },
    { "Pantalón", Image.FromFile(@"C:\Users\car_a\OneDrive\Pictures\producto\—Pngtree—military style cargo pants_21056139.png") },
    { "Zapatillas", Image.FromFile(@"C:\Users\car_a\OneDrive\Pictures\producto\—Pngtree—green transparent sports shoes_9062733.png") }
};


            // Mostrar imagen
            if (imagenesProductos.ContainsKey(producto.Nombre))
                pictureBoxProducto.Image = imagenesProductos[producto.Nombre];
            else
                pictureBoxProducto.Image = null;
        }

        private void buttonCargarImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBoxProducto.Image = Image.FromFile(ofd.FileName);
                    pictureBoxProducto.Tag = ofd.FileName; // guardamos la ruta en Tag
                }
            }
        }
        private void ActualizarTotal()
        {
            decimal total = _productosEnVenta.Sum(p => p.SubTotal);
            labelPrecioDato.Text = total.ToString("N2"); // formato con 2 decimales
        }

        private void BtnAgregarProducto_Click(object sender, EventArgs e)
        {
            // 🔹 Validar nombre y cantidad
            if (string.IsNullOrWhiteSpace(textBoxNombre.Text) ||
                !int.TryParse(textBoxCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese nombre de producto y una cantidad válida (mayor a 0).");
                return;
            }

            // 🔹 Validar precio
            if (!decimal.TryParse(labelPrecioVentaDato.Text, out var precioVenta) || precioVenta <= 0)
            {
                MessageBox.Show("Precio de venta inválido.");
                return;
            }

            // 🔹 Validar stock disponible
            if (!int.TryParse(labelStockDato.Text, out int stockDisponible))
            {
                MessageBox.Show("Stock inválido.");
                return;
            }

            if (cantidad > stockDisponible)
            {
                MessageBox.Show("La cantidad no puede superar el stock disponible.");
                return;
            }

            // 🔹 Verificar si el producto ya existe en la lista
            var productoExistente = _productosEnVenta.FirstOrDefault(p =>
                p.Nombre.Equals(textBoxNombre.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
                p.Talla == comboBoxTalle.Text);

            if (productoExistente != null)
            {
                // Si ya existe, acumular cantidad
                if (productoExistente.Cantidad + cantidad > stockDisponible)
                {
                    MessageBox.Show("La cantidad total supera el stock disponible.");
                    return;
                }

                productoExistente.Cantidad += cantidad;
            }
            else
            {
                // 🔹 Agregar producto nuevo a la lista
                _productosEnVenta.Add(new ProductoVenta
                {
                    IdProducto = _productosEnVenta.Count + 1,
                    Nombre = textBoxNombre.Text.Trim(),
                    Descripcion = textBoxDescripcion.Text.Trim(),
                    Categoria = labelCategoriaDato.Text,
                    Talla = comboBoxTalle.Text,
                    Cantidad = cantidad,
                    PrecioVenta = precioVenta,
                    ImagenRuta = pictureBoxProducto.Tag?.ToString()
                });
            }

            // 🔹 Refrescar grilla y recalcular total
            RefrescarGrilla();
            ActualizarTotal();

            // 🔹 Limpiar campos
            LimpiarCamposProducto();
        }

        private void RefrescarGrilla()
        {
            if (_productosEnVenta.Any())
            {
                dataGridViewDetalle.DataSource = null;
                dataGridViewDetalle.DataSource = _productosEnVenta;
            }
            else
            {
                dataGridViewDetalle.DataSource = null; // limpia la grilla si no hay productos
            }
        }

        private void LimpiarCamposProducto()
        {
            textBoxNombre.Clear();
            textBoxDescripcion.Clear();
            labelStockDato.Text = string.Empty;
            labelPrecioVentaDato.Text = string.Empty;
            comboBoxTalle.SelectedIndex = -1;
            textBoxCantidad.Clear();
        }

        private ClienteVenta ObtenerClienteDesdeCampos()
        {
            var genero = radioButtonHombre.Checked ? "Hombre" :
                         radioButtonMujer.Checked ? "Mujer" : string.Empty;

            return new ClienteVenta
            {
                IdCliente = _clienteSeleccionado?.IdCliente ?? 0,
                Nombre = (guna2TextBox5?.Text ?? string.Empty).Trim(),
                Apellido = (guna2TextBox1?.Text ?? string.Empty).Trim(),
                DNI = (guna2TextBox2?.Text ?? string.Empty).Trim(),
                Correo = (guna2TextBox3?.Text ?? string.Empty).Trim(),
                Telefono = (textBoxTelefono?.Text ?? string.Empty).Trim(),
                Direccion = (guna2TextBox4?.Text ?? string.Empty).Trim(),
                Genero = genero,
                FechaNacimiento = guna2DateTimePicker1?.Value ?? DateTime.MinValue
            };
        }

        private bool ValidarClienteCampos(out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(guna2TextBox5?.Text))
            {
                mensajeError = "Debe ingresar el Nombre del cliente.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(guna2TextBox1?.Text))
            {
                mensajeError = "Debe ingresar el Apellido del cliente.";
                return false;
            }
            return true;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            using (var form = new ClienteNuevo())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var nuevoCliente = form.ClienteCreado;

                    // Guardar en la lista general de clientes
                    RepositorioClientes.Clientes.Add(nuevoCliente);

                    guna2TextBox5.Text = $"{nuevoCliente.Nombre}";
                    guna2TextBox1.Text = $"{nuevoCliente.Apellido}";
                    guna2TextBox2.Text = $"{nuevoCliente.DNI}";
                    textBoxTelefono.Text = $"{nuevoCliente.Telefono}";
                    guna2TextBox4.Text = $"{nuevoCliente.Direccion}";
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (!ValidarClienteCampos(out var error))
            {
                MessageBox.Show(error);
                return;
            }

            if (string.IsNullOrWhiteSpace(comboBoxMetodo?.Text))
            {
                MessageBox.Show("Debe seleccionar un método de pago.");
                return;
            }

            if (_productosEnVenta.Count == 0)
            {
                MessageBox.Show("No hay productos en la venta.");
                return;
            }

            // Tomar cliente desde los campos
            _clienteSeleccionado = ObtenerClienteDesdeCampos();

            // Total por productos agregados
            decimal precioTotal = _productosEnVenta.Sum(p => p.SubTotal);

            var nuevaVenta = new Venta
            {
                IdVenta = _contadorVentas++,
                NombreCliente = $"{_clienteSeleccionado.Nombre} {_clienteSeleccionado.Apellido}",
                DNICliente = _clienteSeleccionado.DNI,
                PrecioTotal = precioTotal,
                FechaVenta = DateTime.Now,
                UsuarioVendedor = SesionActual.Usuario.NombreUsuario // reemplazar por sesión
            };

            _ventas.Add(nuevaVenta);
            MostrarVentas();

            MessageBox.Show($"Venta registrada para {nuevaVenta.NombreCliente} por {nuevaVenta.PrecioTotal:C}");

            // Limpiar productos para la próxima venta, la grilla de ventas queda
            _productosEnVenta.Clear();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridViewVentas.SelectedRows.Count == 0) return;

            int id = (int)dataGridViewVentas.SelectedRows[0].Cells[0].Value;
            var venta = _ventas.FirstOrDefault(v => v.IdVenta == id);
            if (venta != null)
            {
                _ventas.Remove(venta);
            }

            MostrarVentas();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace formulario_de_producto
{
    public partial class Productos : Form
    {
        // Clase interna para representar un producto
        private class Producto
        {
            public int IdProducto { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public decimal PrecioCosto { get; set; }
            public decimal PrecioVenta { get; set; }
            public int Stock { get; set; }
            public int StockMin { get; set; }
            public string Categoria { get; set; }
            public string Talla { get; set; }
            public Image Imagen { get; set; }
            public bool Activo { get; set; }
        }

        // Lista en memoria
        private List<Producto> _productos = new List<Producto>();
        private int? _editandoId = null; // null = alta, valor = id en edición
        private bool? _soloActivos = null; // null = todos, true = activos, false = inactivos

        public Productos()
        {
            InitializeComponent();

            // Eventos
            button7.Click += BtnGuardar_Click;
            button6.Click += BtnEliminar_Click;
            buttonImagen.Click += BtnCargarImagen_Click;
            button4.Click += (s, e) => { _soloActivos = true; MostrarProductos(); };
            button5.Click += (s, e) => { _soloActivos = false; MostrarProductos(); };

            // Nuevo: botón Modificar
            buttonModificar.Click += BtnModificar_Click;
            // Validaciones de entrada numérica
            guna2TextBox2.KeyPress += SoloNumerosDecimales_KeyPress; // Precio Costo
            guna2TextBox1.KeyPress += SoloNumerosDecimales_KeyPress; // Precio Venta
            guna2TextBox3.KeyPress += SoloNumeros_KeyPress;          // Stock Min
            textBoxStock.KeyPress += SoloNumeros_KeyPress;

            this.Load += Productos_Load;
        }

        private void Productos_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            MostrarProductos();
        }

        private void ConfigurarGrilla()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "IdProducto" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "Nombre" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = "Descripcion" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio Costo", DataPropertyName = "PrecioCosto" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio Venta", DataPropertyName = "PrecioVenta" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "Stock" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock Min", DataPropertyName = "StockMin" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Categoría", DataPropertyName = "Categoria" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Talla", DataPropertyName = "Talla" });
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = "Activo" });
        }

        private void MostrarProductos()
        {
            IEnumerable<Producto> lista = _productos;

            if (_soloActivos.HasValue)
                lista = lista.Where(p => p.Activo == _soloActivos.Value);

            dataGridView1.DataSource = lista.ToList();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxNombre.Text) || string.IsNullOrWhiteSpace(textBoxDescripcion.Text))
            {
                MessageBox.Show("Complete al menos nombre y descripción.");
                return;
            }

            int stock = int.TryParse(textBoxStock.Text, out var st) ? st : 0;
            int stockMin = int.TryParse(guna2TextBox3.Text, out var sm) ? sm : 0;

            if (stockMin > stock)
            {
                MessageBox.Show("El stock mínimo no puede ser mayor al stock actual.");
                return;
            }

            if (_editandoId == null)
            {
                // Alta
                int nuevoId = _productos.Count > 0 ? _productos.Max(p => p.IdProducto) + 1 : 1;
                _productos.Add(new Producto
                {
                    IdProducto = nuevoId,
                    Nombre = textBoxNombre.Text.Trim(),
                    Descripcion = textBoxDescripcion.Text.Trim(),
                    PrecioCosto = decimal.TryParse(guna2TextBox2.Text, out var pc) ? pc : 0,
                    PrecioVenta = decimal.TryParse(guna2TextBox1.Text, out var pv) ? pv : 0,
                    Stock = stock,
                    StockMin = stockMin,
                    Categoria = comboBoxCantidad.Text,
                    Talla = guna2ComboBox1.Text.Trim(),
                    Imagen = pictureBox1.Image,
                    Activo = true
                });
            }
            else
            {
                // Edición
                var prod = _productos.FirstOrDefault(p => p.IdProducto == _editandoId.Value);
                if (prod != null)
                {
                    prod.Nombre = textBoxNombre.Text.Trim();
                    prod.Descripcion = textBoxDescripcion.Text.Trim();
                    prod.PrecioCosto = decimal.TryParse(guna2TextBox2.Text, out var pc) ? pc : 0;
                    prod.PrecioVenta = decimal.TryParse(guna2TextBox1.Text, out var pv) ? pv : 0;
                    prod.Stock = stock;
                    prod.StockMin = stockMin;
                    prod.Categoria = comboBoxCantidad.Text;
                    prod.Talla = guna2ComboBox1.Text.Trim();
                    prod.Imagen = pictureBox1.Image;
                }
            }

            LimpiarFormulario();
            MostrarProductos();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            var prod = _productos.FirstOrDefault(p => p.IdProducto == id);
            if (prod != null)
            {
                prod.Activo = false; // Marcamos como inactivo
            }

            MostrarProductos();
        }

        private void BtnCargarImagen_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imágenes|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto para modificar.");
                return;
            }

            int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            var prod = _productos.FirstOrDefault(p => p.IdProducto == id);
            if (prod != null)
            {
                // Cargar datos en los campos
                textBoxNombre.Text = prod.Nombre;
                textBoxDescripcion.Text = prod.Descripcion;
                guna2TextBox2.Text = prod.PrecioCosto.ToString();
                guna2TextBox1.Text = prod.PrecioVenta.ToString();
                guna2TextBox3.Text = prod.StockMin.ToString();
                comboBoxCantidad.Text = prod.Categoria;
                guna2ComboBox1.Text = prod.Talla;
                pictureBox1.Image = prod.Imagen;

                // Guardamos el ID en edición
                _editandoId = prod.IdProducto;
            }
        }

        // Permitir solo números enteros (para stock)
        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // bloquea el caracter
            }
        }

        // Permitir números decimales (para precios)
        private void SoloNumerosDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Permitir control, dígitos y un solo separador decimal
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Evitar más de un separador decimal
            if ((e.KeyChar == ',' || e.KeyChar == '.') && (txt.Text.Contains(",") || txt.Text.Contains(".")))
            {
                e.Handled = true;
            }
        }
        private void LimpiarFormulario()
        {
            textBoxNombre.Clear();
            textBoxDescripcion.Clear();
            guna2TextBox2.Clear();
            guna2TextBox1.Clear();
            guna2TextBox3.Clear();
            comboBoxCantidad.SelectedIndex = -1;
            guna2ComboBox1.SelectedIndex = -1;
            pictureBox1.Image = null;
            _editandoId = null;
        }
    }
}

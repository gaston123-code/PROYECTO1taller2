using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace formulario_cliente
{
    public partial class Cliente : Form
    {
        // Clase interna para representar un cliente
        private class ClienteData
        {
            public int IdCliente { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Direccion { get; set; }
            public string Correo { get; set; }
            public string DNI { get; set; }
            public string Sexo { get; set; }
            public string Telefono { get; set; }
            public DateTime FechaNacimiento { get; set; }
            public bool Activo { get; set; }
        }

        // Lista en memoria
        private List<ClienteData> _clientes = new List<ClienteData>();
        private int? _editandoId = null; // null = alta, valor = id en edición
        private bool? _soloActivos = null; // null = todos, true = activos, false = inactivos

        public Cliente()
        {
            InitializeComponent();

            // Eventos
            button1.Click += BtnGuardar_Click;
            button5.Click += BtnEliminar_Click;
            button4.Click += (s, e) => { _soloActivos = true; MostrarClientes(); };
            button2.Click += (s, e) => { _soloActivos = false; MostrarClientes(); };

            // Nuevo: botón Modificar
            buttonModificar.Click += BtnModificar_Click;

            this.Load += Cliente_Load;

            // Validaciones de entrada
            textBoxNombre.KeyPress += SoloLetras_KeyPress;
            guna2TextBox1.KeyPress += SoloLetras_KeyPress; // Apellido
            guna2TextBox2.KeyPress += SoloNumeros_KeyPress; // DNI
            textBoxTelefono.KeyPress += SoloNumeros_KeyPress; // Teléfono

        }

        // Permitir solo letras, control (borrar, tab) y espacio
        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; // bloquea el caracter
            }
        }

        // Permitir solo números y teclas de control
        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Cliente_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            MostrarClientes();
        }

        private void ConfigurarGrilla()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "IdCliente" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "Nombre" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Apellido", DataPropertyName = "Apellido" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DNI", DataPropertyName = "DNI" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Correo", DataPropertyName = "Correo" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Teléfono", DataPropertyName = "Telefono" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dirección", DataPropertyName = "Direccion" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sexo", DataPropertyName = "Sexo" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha Nac.", DataPropertyName = "FechaNacimiento" });
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = "Activo" });
        }

        private void MostrarClientes()
        {
            IEnumerable<ClienteData> lista = _clientes;

            if (_soloActivos.HasValue)
                lista = lista.Where(c => c.Activo == _soloActivos.Value);

            dataGridView1.DataSource = lista.ToList();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones simples
            if (string.IsNullOrWhiteSpace(textBoxNombre.Text) || string.IsNullOrWhiteSpace(guna2TextBox1.Text))
            {
                MessageBox.Show("Complete al menos nombre y apellido.");
                return;
            }

            if (_editandoId == null)
            {
                // Alta
                int nuevoId = _clientes.Count > 0 ? _clientes.Max(c => c.IdCliente) + 1 : 1;
                _clientes.Add(new ClienteData
                {
                    IdCliente = nuevoId,
                    Nombre = textBoxNombre.Text.Trim(),
                    Apellido = guna2TextBox1.Text.Trim(),
                    Direccion = textBoxDescripcion.Text.Trim(),
                    Correo = guna2TextBox3.Text.Trim(),
                    DNI = guna2TextBox2.Text.Trim(),
                    Sexo = radioButtonHombre.Checked ? "Hombre" : "Mujer",
                    Telefono = textBoxTelefono.Text.Trim(),
                    FechaNacimiento = fecha.Value,
                    Activo = true
                });
            }
            else
            {
                // Edición
                var cli = _clientes.FirstOrDefault(c => c.IdCliente == _editandoId.Value);
                if (cli != null)
                {
                    cli.Nombre = textBoxNombre.Text.Trim();
                    cli.Apellido = guna2TextBox1.Text.Trim();
                    cli.Direccion = textBoxDescripcion.Text.Trim();
                    cli.Correo = guna2TextBox3.Text.Trim();
                    cli.DNI = guna2TextBox2.Text.Trim();
                    cli.Sexo = radioButtonHombre.Checked ? "Hombre" : "Mujer";
                    cli.Telefono = textBoxTelefono.Text.Trim();
                    cli.FechaNacimiento = fecha.Value;
                }
            }

            LimpiarFormulario();
            MostrarClientes();
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;

            int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            var cli = _clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cli != null)
            {
                cli.Activo = false; // Marcamos como inactivo
            }

            MostrarClientes();
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente para modificar.");
                return;
            }

            int id = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            var cli = _clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cli != null)
            {
                // Cargar datos en los campos
                textBoxNombre.Text = cli.Nombre;
                guna2TextBox1.Text = cli.Apellido;
                textBoxDescripcion.Text = cli.Direccion;
                guna2TextBox3.Text = cli.Correo;
                guna2TextBox2.Text = cli.DNI;
                textBoxTelefono.Text = cli.Telefono;
                fecha.Value = cli.FechaNacimiento;

                radioButtonHombre.Checked = cli.Sexo == "Hombre";
                radioButtonMujer.Checked = cli.Sexo == "Mujer";

                // Guardamos el ID en edición
                _editandoId = cli.IdCliente;
            }
        }


        private void LimpiarFormulario()
        {
            textBoxNombre.Clear();
            guna2TextBox1.Clear();
            textBoxDescripcion.Clear();
            guna2TextBox3.Clear();
            guna2TextBox2.Clear();
            radioButtonHombre.Checked = false;
            radioButtonMujer.Checked = false;
            textBoxTelefono.Clear();
            fecha.Value = DateTime.Today;
            _editandoId = null;
        }
    }
}
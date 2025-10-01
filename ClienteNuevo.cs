using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROYECTO1.modelos;

namespace PROYECTO1
{
    public partial class ClienteNuevo : Form
    {
        public ClienteData ClienteCreado { get; private set; }

        public ClienteNuevo()
        {
            InitializeComponent();
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => this.Close();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Debe ingresar al menos nombre y apellido.");
                return;
            }

            ClienteCreado = new ClienteData
            {
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                DNI = txtDNI.Text.Trim(),
                Direccion = txtDirección.Text.Trim(),
                Telefono = txtTelefono.Text.Trim()
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

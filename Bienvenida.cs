using formulario_cliente;
using formulario_de_producto;
using formulario_usuario;
using ProyectoReportes;
using ProyectoVentas;
using System;
using System.Windows.Forms;

namespace PROYECTO1
{
    // formulario principal que contiene el panel para cargar los otros formularios a través de botones
    public partial class FBienvenida : Form
    {
        public FBienvenida()
        {
            InitializeComponent();
            this.Load += FBienvenida_Load;
        }

        private void FBienvenida_Load(object sender, EventArgs e)
        {
            ConfigurarAccesosPorRol();

            // Cargar por defecto el formulario según el rol
            switch (SesionActual.Usuario.IdRol)
            {
                case 1: // Administrador
                    CargarFormularioEnPanel(new Usuarios());
                    break;
                case 2: // Gerente
                    CargarFormularioEnPanel(new Cliente());
                    break;
                case 3: // Vendedor
                    CargarFormularioEnPanel(new Ventas());
                    break;
                default:
                    MessageBox.Show("Rol no reconocido. Contacte al administrador.");
                    break;
            }
        }

        private void ConfigurarAccesosPorRol()
        {
            // Primero deshabilitamos todos
            buttonUsuarios.Enabled = false;
            buttonBackUp.Enabled = false;
            buttonVentas.Enabled = false;
            buttonReportes.Enabled = false;
            buttonClientes.Enabled = false;
            buttonProductos.Enabled = false;

            // Habilitamos según el rol
            switch (SesionActual.Usuario.IdRol)
            {
                case 1: // Administrador
                    buttonUsuarios.Enabled = true;
                    buttonBackUp.Enabled = true;
                    break;
                case 2: // Gerente
                    buttonClientes.Enabled = true;
                    buttonReportes.Enabled = true;
                    buttonProductos.Enabled = true;
                    break;
                case 3: // Vendedor
                    buttonVentas.Enabled = true;
                    break;
            }
        }

        private Form formularioActual;

        private void CargarFormularioEnPanel(Form frm)
        {
            if (formularioActual != null)
            {
                panelContenido.Controls.Remove(formularioActual);
                formularioActual.Close();
                formularioActual.Dispose();
                formularioActual = null;
            }

            formularioActual = frm;
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            panelContenido.Controls.Clear();
            panelContenido.Controls.Add(frm);
            frm.Show();
            frm.BringToFront();
            frm.Focus();
        }

        // eventos click
        private void btnVentas_Click(object sender, EventArgs e)
        {
            CargarFormularioEnPanel(new Ventas());
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            CargarFormularioEnPanel(new Productos());
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            CargarFormularioEnPanel(new Cliente());
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            CargarFormularioEnPanel(new Usuarios());
        }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            CargarFormularioEnPanel(new Reportes());
        }

        private void btnBackUp_Click(object sender, EventArgs e)
        {
            CargarFormularioEnPanel(new BackUp());
        }
    }
}

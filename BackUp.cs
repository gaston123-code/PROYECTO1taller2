using System;
using System.Windows.Forms;

namespace PROYECTO1
{
    public partial class BackUp : Form
    {
        public BackUp()
        {
            InitializeComponent();

            // Enganchar eventos
            buttonConectar.Click += BtnConectar_Click;
            buttonRuta.Click += BtnRuta_Click;
            buttonBackUp.Click += BtnBackup_Click;
        }

        private void BtnConectar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBoxBaseDatos.Text))
            {
                MessageBox.Show("Ingrese el nombre de la base de datos.");
                return;
            }

            // Simulación de conexión
            MessageBox.Show($"Conectado a la base de datos: {comboBoxBaseDatos.Text}");
        }

        private void BtnRuta_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBoxRuta.Text = fbd.SelectedPath;
                }
            }
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBoxBaseDatos.Text))
            {
                MessageBox.Show("Debe ingresar la base de datos.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxRuta.Text))
            {
                MessageBox.Show("Debe seleccionar una ruta de destino.");
                return;
            }

            // Simulación de backup
            string archivo = System.IO.Path.Combine(textBoxRuta.Text, $"{comboBoxBaseDatos.Text}_backup_{DateTime.Now:yyyyMMddHHmmss}.bak");
            MessageBox.Show($"Backup simulado creado en:\n{archivo}");
        }
    }
}

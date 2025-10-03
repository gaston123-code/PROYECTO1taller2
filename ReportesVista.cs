using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO1
{
    public partial class ReportesVista : Form
    {
        public ReportesVista(string titulo, string contenido)
        {
            InitializeComponent();
            this.Text = titulo; // título de la ventana
            richTextBox1.ReadOnly = true;
            richTextBox1.Text = contenido;

        }
    }
}

using formulario_usuario;
using PROYECTO1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto
{
    public partial class InicioSesion : Form
    {
       
        public UsuarioSesion UsuarioLogueado { get; private set; } //objeto de tipo UsuarioSesion que guarda el usuario logueado
        public InicioSesion()
        {
            InitializeComponent();

            this.AcceptButton = buttonIngresar; // presionar Enter activa el boton ingresar, se le asocia un evento
            this.buttonIngresar.Click += buttonIngresar_Click; //se asocia el evento click del boton ingresar al metodo buttonIngresar_Click
        }

        private void buttonIngresar_Click(object sender, EventArgs e) //metodo que se ejecuta al hacer click en el boton ingresar
        {
            string u = textBoxUsuario.Text.Trim(); //se guarda el usuario en la variable u
            string p = textBoxContraseña.Text; //se guarda la contraseña en la variable p

            if (string.IsNullOrWhiteSpace(u) || string.IsNullOrWhiteSpace(p)) //verifica que usuario y contraseña no esten vacios
            {
                MessageBox.Show("Ingresá usuario y contraseña.");
                return;
            }

            // consulta SQL para verificar usuario u y contraseña p. Solo permite usuarios activos
            const string sql = @"
SELECT TOP 1 id_usuario, nombre_usuario, nombre, apellido, Id_rol
FROM dbo.usuario
WHERE nombre_usuario = @u AND [contraseña] = @p AND (estado = 1 OR estado IS NULL);";

            try // manejo de errores
            {
                using (var cn = BaseDatos.GetConnection())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@u", u);
                    cmd.Parameters.AddWithValue("@p", p);

                    cn.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.");
                            return;
                        }

                        // crea un objeto de tipo UsuarioSesion con los datos del usuario logueado
                        UsuarioLogueado = new UsuarioSesion
                        {
                            Id = rd.GetInt32(rd.GetOrdinal("id_usuario")),
                            NombreUsuario = rd.GetString(rd.GetOrdinal("nombre_usuario")),
                            Nombre = rd.GetString(rd.GetOrdinal("nombre")),
                            Apellido = rd.GetString(rd.GetOrdinal("apellido")),
                            IdRol = rd.GetInt32(rd.GetOrdinal("Id_rol"))
                        };
                    }
                }

                // guarda el usuario logueado en la variable Usuario para usarlo en toda la app
                SesionActual.Usuario = UsuarioLogueado;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            //si algo falla, muestra error
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar sesión.\n" + ex.Message);
            }
        }

    }
}

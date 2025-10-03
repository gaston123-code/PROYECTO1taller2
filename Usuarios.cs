using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using PROYECTO1;
using System.Collections.Generic;

namespace formulario_usuario
{
    public partial class Usuarios : Form
    {
        //variables privadas que se usan para manejar la lógica interna del formulario
        private readonly BindingSource _bsUsuarios = new BindingSource(); //fuente de datos para enlazar la tabla de usuarios con el DataGridView
        private DataTable _dtUsuarios; //tabla en memoria para almacenar los datos de los usuarios
        private bool? _soloActivos = null; //filtro para mostrar solo usuarios activos, inactivos o todos
        private string _filtroTexto = null;  //filtro de texto para buscar en la tabla
        private int? _editandoId = null;   //id del usuario que se está editando, null si no se está editando


        public Usuarios() //constructor del formulario
        {
            InitializeComponent();

            this.Load += Usuarios_Load; //evento que se ejecuta al cargar el formulario

            ConfigurarGrilla(); //configura las columnas de la grilla
        }

        private void Usuarios_Load(object sender, EventArgs e) //método que se ejecuta al cargar el formulario
        {
            CargarRolesEnCombo(); 
            CargarUsuariosEnGrilla(); 
            ConfigurarBuscador();
            // dos eventos para que el checkbox “Actividad” actualice la BD al tildar/destildar
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

        }

        private void ConfigurarGrilla() //configura las columnas de la grilla
        {
            dataGridView1.AutoGenerateColumns = false; //no genera columnas automáticamente

            // Esto hace que cada columna de la grilla se llene automáticamente desde el DataTable que trae el SELECT.
            IdUsuario.DataPropertyName = "id_usuario";
            IdRol.DataPropertyName = "Id_rol";
            Nombre.DataPropertyName = "nombre";
            Apellido.DataPropertyName = "apellido";
            Usuario.DataPropertyName = "nombre_usuario";
            DNI.DataPropertyName = "DNI_usuario";
            Email.DataPropertyName = "correo";
            Telefono.DataPropertyName = "teléfono";            
            Dirección.DataPropertyName = "Dirección";           
            Sexo.DataPropertyName = "genero";
            Fecha.DataPropertyName = "fecha_de_nacimiento";
            Actividad.DataPropertyName = "Activo";              
        }

        private void CargarRolesEnCombo() //carga los roles en el comboBox
        {
            try 
            {
                //abre una conexion a la base de datos y carga los roles en el comboBox
                using (var cn = BaseDatos.GetConnection())
                {
                    cn.Open();
                    var roles = new[] { "Administrador", "Gerente", "Vendedor" };
                    foreach (var r in roles)
                    {
                        // inserta el rol en la tabla rol si no existe en la base de datos
                        using (var cmdIns = new SqlCommand(
                            "IF NOT EXISTS (SELECT 1 FROM dbo.rol WHERE nombre=@n) INSERT INTO dbo.rol(nombre) VALUES(@n);", cn))
                        {
                            cmdIns.Parameters.AddWithValue("@n", r);
                            cmdIns.ExecuteNonQuery();
                        }
                    }

                    // carga los roles desde la base de datos al comboBox
                    using (var da = new SqlDataAdapter("SELECT Id_rol, nombre FROM dbo.rol ORDER BY nombre;", cn))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        comboBoxRol.DisplayMember = "nombre";
                        comboBoxRol.ValueMember = "Id_rol";
                        comboBoxRol.DataSource = dt;
                    }
                }
                comboBoxRol.SelectedIndex = -1; //ningun rol seleccionado por defecto
                comboBoxRol.Text = ""; //sin texto por defecto
                comboBoxRol.DropDownStyle = ComboBoxStyle.DropDownList; //solo se puede seleccionar, no escribir
            }

            // si hay un error, muestra un mensaje y carga los roles por defecto
            catch (Exception ex)
            {
                MessageBox.Show("No pude cargar roles.\n" + ex.Message);
                comboBoxRol.DataSource = null;
                comboBoxRol.Items.Clear();
                comboBoxRol.Items.AddRange(new object[] { "Administrador", "Gerente", "Vendedor" });
                comboBoxRol.SelectedIndex = 0;
            }
        }


        
        private void CargarUsuariosEnGrilla() //muestra los usuarios en la grilla
        {

            //Ejecuta una consulta SQL para traer todos los usuarios desde la base de datos
            const string sql = @"
SELECT 
    u.id_usuario,
    u.Id_rol,
    u.nombre,
    u.apellido,
    u.nombre_usuario,
    u.DNI_usuario,
    u.correo,
    u.[teléfono],
    u.[Dirección],
    u.[contraseña],
    u.genero,
    u.fecha_de_nacimiento,
    CAST(CASE WHEN u.estado = 1 THEN 1 ELSE 0 END AS bit) AS Activo
FROM dbo.usuario u
ORDER BY u.id_usuario DESC;";


            //Se abre una conexión a la base de datos y se guarda el resultado en un DataTable
            try
            {
                using (var cn = BaseDatos.GetConnection())
                using (var da = new SqlDataAdapter(sql, cn))
                {
                    _dtUsuarios = new DataTable();
                    da.Fill(_dtUsuarios);

                    //se cargan los datos en el BindingSource y se enlaza con la grilla
                    _bsUsuarios.DataSource = _dtUsuarios;
                    dataGridView1.DataSource = _bsUsuarios;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No pude cargar usuarios.\n" + ex.Message);
            }
        }



        //verifica si ya existe un usuario con el mismo nombre o dni
        private bool ExisteUsuario(string nombreUsuario, int dni)
        {

            //consulta SQL para buscar un usuario con el mismo nombre o dni
            const string sql = "SELECT 1 FROM dbo.usuario WHERE nombre_usuario=@u OR DNI_usuario=@dni;";
            try
            {
                using (var cn = BaseDatos.GetConnection())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@u", nombreUsuario);
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cn.Open();
                    var r = cmd.ExecuteScalar();
                    return r != null;
                }
            }
            catch
            {
                
                return false;
            }
        }

        private void LimpiarFormulario()
        {
            textBoxNombre.Clear();
            textBoxApellido.Clear();
            textBoxUsuario.Clear();
            textBoxContraseña.Clear();
            textBoxDNI.Clear();
            textBoxEmail.Clear();
            textBoxDirección.Clear();
            textBoxTelefono.Clear();
            dateTimePickerNacimiento.Value = DateTime.Today;
            radioButtonHombre.Checked = false;
            radioButtonMujer.Checked = false;

            if (comboBoxRol.Items.Count > 0) comboBoxRol.SelectedIndex = 0;
        }

        // botones para filtrar por activos, inactivos o todos
        private void buttonMostrarActivos_Click(object sender, EventArgs e)
        {
            ToggleFiltroEstado(true);
        }

        private void buttonMostrarInactivos_Click(object sender, EventArgs e)
        {
            ToggleFiltroEstado(false);
        }

        private void ToggleFiltroEstado(bool activos)
        {
            
            if (_soloActivos.HasValue && _soloActivos.Value == activos)
                _soloActivos = null;
            else
                _soloActivos = activos;

            AplicarFiltrosCombinados();
        }

        // Aplica el filtro de texto y el filtro de estado para que s epeudan usar al mismo tiempo
        private void AplicarFiltro()
        {
            if (_bsUsuarios.DataSource == null || comboBoxColumna.SelectedValue == null)
            {
                _filtroTexto = null;
                AplicarFiltrosCombinados();
                return;
            }

            var col = comboBoxColumna.SelectedValue.ToString();
            var txt = (textBoxBuscador.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(txt))
            {
                _filtroTexto = null;  
                AplicarFiltrosCombinados();
                return;
            }

            var safe = EscapeLikeValue(txt);
            _filtroTexto = $"CONVERT([{col}], 'System.String') LIKE '%{safe}%'";
            AplicarFiltrosCombinados();
        }

        private void AplicarFiltrosCombinados()
        {
            var partes = new List<string>();

            if (!string.IsNullOrEmpty(_filtroTexto))
                partes.Add(_filtroTexto);

            if (_soloActivos.HasValue)
                partes.Add(_soloActivos.Value ? "Activo = True" : "Activo = False");

            _bsUsuarios.Filter = partes.Count > 0 ? string.Join(" AND ", partes) : string.Empty;
        }


        private void textBoxNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; 
            }
        }


        private void textBoxNombre_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;

            if (!string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = char.ToUpper(txt.Text[0]) + txt.Text.Substring(1);
                txt.SelectionStart = txt.Text.Length; 
            }
        }

        
        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }

        private void buttonGuardar_Click(object sender, EventArgs e) // metodo que se ejecuta al hacer click en el boton guardar
        {
            //valida que todos los campos sean validos
            if (string.IsNullOrWhiteSpace(textBoxNombre.Text) ||
                string.IsNullOrWhiteSpace(textBoxApellido.Text) ||
                string.IsNullOrWhiteSpace(textBoxUsuario.Text) ||
                string.IsNullOrWhiteSpace(textBoxContraseña.Text) ||
                string.IsNullOrWhiteSpace(textBoxDNI.Text) ||
                string.IsNullOrWhiteSpace(textBoxDirección.Text) ||
                string.IsNullOrWhiteSpace(textBoxTelefono.Text) ||
                (!radioButtonHombre.Checked && !radioButtonMujer.Checked) ||
                comboBoxRol.SelectedIndex == -1)
            {
                MessageBox.Show("Error, complete todos los campos.");
                return;
            }

            if (!int.TryParse(textBoxDNI.Text.Trim(), out int dni))
            {
                MessageBox.Show("DNI inválido.");
                return;
            }

            string genero = radioButtonHombre.Checked ? "Hombre" : "Mujer";
            int idRol = 1;
            if (comboBoxRol.SelectedValue != null)
                int.TryParse(comboBoxRol.SelectedValue.ToString(), out idRol);

            // Si se está editando un usuario, actualiza sus datos
            if (_editandoId.HasValue)
            {
                
                if (ExisteUsuarioDuplicado(textBoxUsuario.Text.Trim(), dni, _editandoId.Value))
                {
                    MessageBox.Show("Ya existe un usuario con ese nombre o DNI.");
                    return;
                }

                // actualiza el usuario en la base de datos con un UPDATE
                const string sqlUpdate = @"
UPDATE dbo.usuario
   SET DNI_usuario=@dni,
       [contraseña]=@pass,
       nombre=@nom,
       apellido=@ape,
       [teléfono]=@tel,
       [Dirección]=@dir,
       nombre_usuario=@user,
       fecha_de_nacimiento=@fec,
       Id_rol=@rol,
       correo=@correo,
       genero=@genero
 WHERE id_usuario=@id;";

                try
                {
                    using (var cn = BaseDatos.GetConnection())
                    using (var cmd = new SqlCommand(sqlUpdate, cn))
                    {
                        cmd.Parameters.AddWithValue("@dni", dni);
                        cmd.Parameters.AddWithValue("@pass", textBoxContraseña.Text);
                        cmd.Parameters.AddWithValue("@nom", textBoxNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@ape", textBoxApellido.Text.Trim());
                        cmd.Parameters.AddWithValue("@tel", textBoxTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@dir", textBoxDirección.Text.Trim());
                        cmd.Parameters.AddWithValue("@user", textBoxUsuario.Text.Trim());
                        cmd.Parameters.AddWithValue("@fec", dateTimePickerNacimiento.Value.Date);
                        cmd.Parameters.AddWithValue("@rol", idRol);
                        cmd.Parameters.AddWithValue("@correo", textBoxEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@genero", genero);
                        cmd.Parameters.AddWithValue("@id", _editandoId.Value);

                        cn.Open();
                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                        {
                            MessageBox.Show("Usuario actualizado correctamente.");
                            SalirModoEdicion();
                            CargarUsuariosEnGrilla();   
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el usuario.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar:\n" + ex.Message);
                }

                return;
            }

            
            if (ExisteUsuario(textBoxUsuario.Text.Trim(), dni))
            {
                MessageBox.Show("Ya existe un usuario con ese nombre o DNI.");
                return;
            }

            // inserta un nuevo usuario en la base de datos con un INSERT
            const string sqlInsert = @"
INSERT INTO dbo.usuario
 (DNI_usuario, [contraseña], nombre, apellido, [teléfono], [Dirección],
  nombre_usuario, fecha_de_nacimiento, Id_rol, estado, correo, genero)
VALUES
 (@dni, @pass, @nom, @ape, @tel, @dir, @user, @fec, @rol, @estado, @correo, @genero);";

            try
            {
                using (var cn = BaseDatos.GetConnection())
                using (var cmd = new SqlCommand(sqlInsert, cn))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@pass", textBoxContraseña.Text);
                    cmd.Parameters.AddWithValue("@nom", textBoxNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@ape", textBoxApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@tel", textBoxTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@dir", textBoxDirección.Text.Trim());
                    cmd.Parameters.AddWithValue("@user", textBoxUsuario.Text.Trim());
                    cmd.Parameters.AddWithValue("@fec", dateTimePickerNacimiento.Value.Date);
                    cmd.Parameters.AddWithValue("@rol", idRol);
                    cmd.Parameters.AddWithValue("@estado", 1); 
                    cmd.Parameters.AddWithValue("@correo", textBoxEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@genero", genero);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Usuario insertado correctamente.");
                LimpiarFormulario();
                CargarUsuariosEnGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar:\n" + ex.Message);
            }
        }




        //Crea una lista de columnas que el usuario puede elegir para buscar y las carga en el combobox
        private void ConfigurarBuscador()
        {
            // lista de columnas para el comboBox
            var columnas = new List<KeyValuePair<string, string>>
    {
        new("Id Usuario",     "id_usuario"),
        new("Id Rol",         "Id_rol"),
        new("Nombre",         "nombre"),
        new("Apellido",       "apellido"),
        new("Usuario",        "nombre_usuario"),
        new("DNI",            "DNI_usuario"),
        new("Email",          "correo"),
        new("Telefono",       "teléfono"), 
        new("Dirección",  "Dirección")  
    };

            // configura el comboBox con las columnas
            comboBoxColumna.DataSource = columnas;
            comboBoxColumna.DisplayMember = "Key";
            comboBoxColumna.ValueMember = "Value";
            comboBoxColumna.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxColumna.SelectedIndex = 0; 

            
            comboBoxColumna.SelectedIndexChanged += (_, __) => AplicarFiltro();
            textBoxBuscador.TextChanged += (_, __) => AplicarFiltro();
        }

        // Aplica el filtro de texto según la columna seleccionada y el texto ingresado
        private void AplicarFiltro(object sender, EventArgs e)
        {
            // si no hay datos o no hay columna seleccionada, quita el filtro
            if (_bsUsuarios.DataSource == null || comboBoxColumna.SelectedValue == null)
            {
                _bsUsuarios.RemoveFilter();
                return;
            }

            var col = comboBoxColumna.SelectedValue.ToString();   
            var txt = (textBoxBuscador.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(txt))
            {
                _bsUsuarios.RemoveFilter(); 
                return;
            }

            var safe = EscapeLikeValue(txt);

            // aplica el filtro de texto en la columna seleccionada
            _bsUsuarios.Filter = $"CONVERT([{col}], 'System.String') LIKE '%{safe}%'";
        }


        // función que prepara el texto del buscador para que no cause errores cuando se usa en un filtro SQL con LIKE
        private static string EscapeLikeValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return value
                .Replace("[", "[[]")
                .Replace("]", "[]]")
                .Replace("%", "[%]")
                .Replace("_", "[_]")
                .Replace("'", "''");
        }



        // Maneja el cambio de estado de la celda actual para actualizar la base de datos cuando se cambia el checkbox "Actividad"
        private bool _toggleBusy = false;

        // indica que el usuario modificó la celda
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty &&
                dataGridView1.CurrentCell != null &&
                dataGridView1.CurrentCell.OwningColumn != null &&
                dataGridView1.CurrentCell.OwningColumn.Name == "Actividad")
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // actualiza la base de datos cuando se cambia el valor del checkbox "Actividad"
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_toggleBusy) return;
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dataGridView1.Columns[e.ColumnIndex].Name != "Actividad") return;

            var row = dataGridView1.Rows[e.RowIndex];

            
            if (row.Cells["IdUsuario"].Value == null) return;
            if (!int.TryParse(row.Cells["IdUsuario"].Value.ToString(), out int idUsuario)) return;

            
            bool activo = false;
            var cellVal = row.Cells["Actividad"].Value;
            if (cellVal != null && cellVal != DBNull.Value)
                activo = Convert.ToBoolean(cellVal);

            try
            {
                using (var cn = BaseDatos.GetConnection())
                using (var cmd = new SqlCommand("UPDATE dbo.usuario SET estado=@est WHERE id_usuario=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@est", activo ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                
            }
            catch (Exception ex)
            {
                
                try
                {
                    _toggleBusy = true;
                    row.Cells["Actividad"].Value = !activo;
                }
                finally
                {
                    _toggleBusy = false;
                }
                MessageBox.Show("No se pudo cambiar la actividad del usuario.\n" + ex.Message);
            }
        }

        private void buttonLimpiar_Click(object sender, EventArgs e)
        {
            textBoxNombre.Clear();
            textBoxApellido.Clear();
            textBoxUsuario.Clear();
            textBoxContraseña.Clear();
            textBoxDNI.Clear();
            textBoxEmail.Clear();
            textBoxDirección.Clear();
            textBoxTelefono.Clear();
            dateTimePickerNacimiento.Value = DateTime.Today;
            radioButtonHombre.Checked = false;
            radioButtonMujer.Checked = false;

            if (comboBoxRol.Items.Count > 0) comboBoxRol.SelectedIndex = 0;
        }


        // botón para modificar un usuario seleccionado en la grilla
        private void buttonModificar_Click(object sender, EventArgs e)
        {

            // si no hay fila seleccionada, muestra un mensaje y sale
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná una fila.");
                return;
            }

            var row = dataGridView1.CurrentRow;

            // si no se puede obtener el id del usuario, muestra un mensaje y sale
            if (row.Cells["IdUsuario"].Value == null ||
                !int.TryParse(row.Cells["IdUsuario"].Value.ToString(), out var id))
            {
                MessageBox.Show("No pude obtener el Id del usuario.");
                return;
            }

            _editandoId = id;

            // carga los datos del usuario en el formulario para editar
            textBoxNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
            textBoxApellido.Text = row.Cells["Apellido"].Value?.ToString() ?? "";
            textBoxUsuario.Text = row.Cells["Usuario"].Value?.ToString() ?? "";
            textBoxDNI.Text = row.Cells["DNI"].Value?.ToString() ?? "";
            textBoxEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            textBoxTelefono.Text = row.Cells["Telefono"].Value?.ToString() ?? "";
            textBoxDirección.Text = row.Cells["Dirección"].Value?.ToString() ?? "";
            var drv = row.DataBoundItem as DataRowView;
            textBoxContraseña.Text = drv?["contraseña"]?.ToString() ?? "";


            if (DateTime.TryParse(row.Cells["Fecha"].Value?.ToString(), out var fnac))
                dateTimePickerNacimiento.Value = fnac;
            else
                dateTimePickerNacimiento.Value = DateTime.Today;

            
            var sexo = row.Cells["Sexo"].Value?.ToString();
            radioButtonHombre.Checked = string.Equals(sexo, "Hombre", StringComparison.OrdinalIgnoreCase);
            radioButtonMujer.Checked = string.Equals(sexo, "Mujer", StringComparison.OrdinalIgnoreCase);

            
            if (int.TryParse(row.Cells["IdRol"].Value?.ToString(), out var idRol))
                comboBoxRol.SelectedValue = idRol;
            else
                comboBoxRol.SelectedIndex = -1;

            
            buttonGuardar.Text = "        ACTUALIZAR";
            labelUsuario1.Text = "EDITANDO";
        }

        // verifica si ya existe otro usuario con el mismo nombre o dni
        private bool ExisteUsuarioDuplicado(string nombreUsuario, int dni, int idIgnorar)
        {
            const string sql = @"
SELECT 1
  FROM dbo.usuario
 WHERE (nombre_usuario=@u OR DNI_usuario=@dni)
   AND id_usuario <> @id;";

            try
            {
                using (var cn = BaseDatos.GetConnection())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@u", nombreUsuario);
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@id", idIgnorar);
                    cn.Open();
                    var r = cmd.ExecuteScalar();
                    return r != null;
                }
            }
            catch
            {
                
                return false;
            }
        }

        // sale del modo edición y vuelve al modo inserción
        private void SalirModoEdicion()
        {
            _editandoId = null;
            buttonGuardar.Text = "       GUARDAR";
            labelUsuario1.Text = "USUARIO";
            LimpiarFormulario();
        }


    }
}

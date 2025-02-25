using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto1
{
    public partial class Admin : Form
    {
        public int idUsuario { get; set; }

        public string UsuarioActual { get; set; }

        public Admin(int id, string usuario)
        {
            InitializeComponent();

            idUsuario = id;
            UsuarioActual = usuario;

            CargarDatosUsuario();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void CargarDatosUsuario()
        {
            usuarioToolStripMenuItem.Text = UsuarioActual;
        }

        private void cerrarSesionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            // Confirmar cierre de sesión
            DialogResult confirmacion = MessageBox.Show(
                "¿Estás seguro de que deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                this.Close(); // Cierra Form1

                // Reabre Form3
                InicioSesion inicioSesion = new InicioSesion();
                inicioSesion.Show();
            }
        }

        private void inicioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            this.Hide();

            // Abrir Form3 (Ventana Home)
            Home home = new Home(idUsuario, UsuarioActual, true);
            home.Show();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void mostarUsuarios()
        {
            textBox1.Clear();

            string datos = "Server=localhost;Port=3306;" +
                           "Database=prueba_desarrollo_interfaces;" +
                           "User ID=root;" +
                           "Password=1234;" +
                           "Pooling=true;";
            string query = "SELECT id, nombre, contraseña, administrador, estado FROM usuarios";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(datos))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dataGridView1.AutoGenerateColumns = true;
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mostarUsuarios();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            AñadirUsuario añadirUsuario = new AñadirUsuario();
            añadirUsuario.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mostarUsuarios();

            textBox1.Clear();

            button5.Visible = true;

            dataGridView1.ReadOnly = false; // Permitir edición en todo el DataGridView
            dataGridView1.AllowUserToAddRows = false; // Evitar filas nuevas vacías
            dataGridView1.AllowUserToDeleteRows = false; // Evitar eliminación directa
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter; // Iniciar edición al hacer clic en una celda
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect; // Seleccionar celdas individuales

            // Asegurar que todas las columnas sean editables
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.ReadOnly = false;
            }

            dataGridView1.Columns["id"].ReadOnly = true;

            // Suscribir eventos para cambiar el color de fondo
            dataGridView1.CellEnter += DataGridView1_CellEnter;
            dataGridView1.CellLeave += DataGridView1_CellLeave;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            EliminarUsuario eliminarUsuario = new EliminarUsuario();
            eliminarUsuario.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            string datos = "Server=localhost;Port=3306;" +
                   "Database=prueba_desarrollo_interfaces;" +
                   "User ID=root;" +
                   "Password=1234;" +
                   "Pooling=true;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(datos))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Obtener los datos de las columnas
                        int id = Convert.ToInt32(row.Cells["id"].Value);
                        string nombre = row.Cells["nombre"].Value.ToString();
                        string contraseña = row.Cells["contraseña"].Value.ToString();
                        string administrador = row.Cells["administrador"].Value.ToString();
                        string estado = row.Cells["estado"].Value.ToString();

                        // Validación de campos vacíos
                        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contraseña) ||
                            string.IsNullOrWhiteSpace(administrador) || string.IsNullOrWhiteSpace(estado))
                        {
                            MessageBox.Show("Rellena todos los campos.");
                            return;
                        }

                        // Validar que el nombre solo contenga letras, no tenga más de 20 caracteres y comience con mayúscula
                        if (!System.Text.RegularExpressions.Regex.IsMatch(nombre, @"^[A-Z][a-zA-Z]*$"))
                        {
                            MessageBox.Show("El nombre debe comenzar con una letra mayúscula y solo puede contener letras.");
                            return;
                        }

                        if (nombre.Length > 20)
                        {
                            MessageBox.Show("El nombre no puede tener más de 20 caracteres.");
                            return;
                        }

                        // Validar que la contraseña tenga entre 4 y 16 caracteres
                        if (contraseña.Length < 4 || contraseña.Length > 16)
                        {
                            MessageBox.Show("La contraseña debe tener entre 4 y 16 caracteres.");
                            return;
                        }

                        // Validar que "administrador" sea "true" o "false"
                        if (!administrador.Equals("Si", StringComparison.OrdinalIgnoreCase) &&
                            !administrador.Equals("No", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("El campo 'administrador' debe ser 'Si' o 'No'.");
                            return;
                        }

                        // Validar que "estado" sea "activo" o "baneado"
                        if (!estado.Equals("Activo", StringComparison.OrdinalIgnoreCase) &&
                            !estado.Equals("Baneado", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("El campo 'estado' debe ser 'Activo' o 'Baneado'.");
                            return;
                        }

                        // Verificar si el ID ya existe para otro usuario (permitir el ID del usuario actual)
                        /*string queryCheckID = "SELECT COUNT(*) FROM usuarios WHERE id = @id AND id != @currentId";
                        using (MySqlCommand checkIDCommand = new MySqlCommand(queryCheckID, connection))
                        {
                            checkIDCommand.Parameters.AddWithValue("@id", id);
                            checkIDCommand.Parameters.AddWithValue("@currentId", row.Cells["id"].Value);  // Asegura que no se valide el ID actual
                            int idExists = Convert.ToInt32(checkIDCommand.ExecuteScalar());

                            if (idExists > 0)
                            {
                                MessageBox.Show($"El ID '{id}' ya existe. Por favor, elige un ID diferente.");
                                return; // Salir sin guardar cambios
                            }
                        }*/

                        // Verificar si la contraseña ya existe para otro usuario
                        string queryCheckPassword = "SELECT COUNT(*) FROM usuarios WHERE contraseña = @contraseña AND id != @currentId";
                        using (MySqlCommand checkPasswordCommand = new MySqlCommand(queryCheckPassword, connection))
                        {
                            checkPasswordCommand.Parameters.AddWithValue("@contraseña", contraseña);
                            checkPasswordCommand.Parameters.AddWithValue("@currentId", row.Cells["id"].Value);
                            int passwordExists = Convert.ToInt32(checkPasswordCommand.ExecuteScalar());

                            if (passwordExists > 0)
                            {
                                MessageBox.Show($"La contraseña ingresada ya está siendo utilizada por otro usuario. Por favor, elige una diferente.");
                                return; // Salir sin guardar cambios
                            }
                        }

                        // Si el ID y la contraseña son válidos, ejecutar la consulta UPDATE
                        string query = "UPDATE usuarios SET nombre = @nombre, contraseña = @contraseña, " +
                                       "administrador = @administrador, estado = @estado WHERE id = @id";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.Parameters.AddWithValue("@nombre", nombre);
                            command.Parameters.AddWithValue("@contraseña", contraseña);
                            command.Parameters.AddWithValue("@administrador", administrador);
                            command.Parameters.AddWithValue("@estado", estado);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                MessageBox.Show("No se pudo actualizar el usuario. Verifique los datos.");
                                return;
                            }
                        }
                    }
                }

                // Volver a cargar los datos en el DataGridView
                button5.Visible = false;
                MessageBox.Show("Cambios guardados correctamente.");

                // Recargar datos manualmente para reflejar los cambios
                string queryReload = "SELECT id, nombre, contraseña, administrador, estado FROM usuarios";
                MySqlDataAdapter adapter = new MySqlDataAdapter(queryReload, datos);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;

                // Deshabilitar la edición
                dataGridView1.ReadOnly = true;

                // Desuscribir eventos para cambiar el color
                dataGridView1.CellEnter -= DataGridView1_CellEnter;
                dataGridView1.CellLeave -= DataGridView1_CellLeave;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los cambios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento para cambiar fondo a gris claro al seleccionar una celda
        private void DataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGray;
            }
        }

        // Evento para restaurar fondo blanco al deseleccionar una celda
        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string buscar = textBox1.Text.Trim();  // Obtén el texto de búsqueda

            if (string.IsNullOrEmpty(buscar))
            {
                MessageBox.Show("Por favor, ingrese un valor para buscar.");
                return;
            }

            // Limpiar el DataGridView antes de mostrar los resultados
            dataGridView1.DataSource = null;

            string datos = "Server=localhost;Port=3306;" +
                           "Database=prueba_desarrollo_interfaces;" +
                           "User ID=root;" +
                           "Password=1234;" +
                           "Pooling=true;";

            // Crear la consulta SQL para buscar por id o nombre
            string query = "SELECT id, nombre, contraseña, administrador, estado FROM usuarios " +
                           "WHERE id LIKE @buscar OR nombre LIKE @buscar";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(datos))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@buscar", "%" + buscar + "%");  // Parametrización para evitar inyecciones SQL

                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Asignar los resultados al DataGridView
                        dataGridView1.DataSource = dt;

                        // Si no se encuentran resultados
                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("No se encontraron resultados.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el usuario: {ex.Message}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();

            // Crear una instancia del formulario de gestión de juegos
            GestionJuegos gestionJuegos = new GestionJuegos(idUsuario, UsuarioActual);
            gestionJuegos.Show();
        }
    }
}

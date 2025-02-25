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
    public partial class GestionJuegos : Form
    {
        public int idUsuario { get; set; }

        public string UsuarioActual { get; set; }
        private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";
        private JuegoControl juegoControlActual;

        public GestionJuegos(int id, string usuario)
        {
            InitializeComponent();

            idUsuario = id;
            UsuarioActual = usuario;

            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            usuarioToolStripMenuItem.Text = UsuarioActual;
        }

        //boton mostrar juegos
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "SELECT id, titulo, descripcion, precio, imagen, categoria FROM juegos";
                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los juegos: {ex.Message}");
            }
        }

        private void AbrirJuegoControl(string modo)
        {
            textBox1.Clear();

            // Cerrar y eliminar el control anterior si existe
            if (juegoControlActual != null)
            {
                this.Controls.Remove(juegoControlActual);
                juegoControlActual.Dispose();
            }

            // Crear una nueva instancia y guardarla
            juegoControlActual = new JuegoControl();
            juegoControlActual.ConfigurarModo(modo);

            // Agregar el nuevo UserControl dentro del formulario
            this.Controls.Add(juegoControlActual);
            juegoControlActual.BringToFront();
        }

        //boton añadir juego
        private void button2_Click(object sender, EventArgs e)
        {
            AbrirJuegoControl("añadir");
        }

        //boton editar juego
        private void button3_Click(object sender, EventArgs e)
        {
            AbrirJuegoControl("editar");
        }

        //boton eliminar juego
        private void button4_Click(object sender, EventArgs e)
        {
            AbrirJuegoControl("eliminar");
        }

        //boton buscar juego
        private void button6_Click(object sender, EventArgs e)
        {
            // Obtener el valor del TextBox para buscar por ID o título
            string juego = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(juego))
            {
                MessageBox.Show("Por favor, ingrese un ID o un título para buscar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();

                    // Modificar la consulta para buscar por ID o título
                    string query = "SELECT id, titulo, descripcion, precio, imagen, categoria FROM juegos WHERE id = @juego OR titulo LIKE @juego";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        // Si el término de búsqueda es un número, lo tratamos como ID (para buscar por ID)
                        if (int.TryParse(juego, out int id))
                        {
                            command.Parameters.AddWithValue("@juego", id);
                        }
                        else
                        {
                            // Si es texto, se busca por título (haciendo LIKE para encontrar coincidencias)
                            command.Parameters.AddWithValue("@juego", "%" + juego + "%");
                        }

                        // Ejecutar la consulta y llenar el DataGridView con los resultados
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader); // Llenar el DataTable con los resultados
                            dataGridView1.DataSource = dt; // Asignar el resultado al DataGridView
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el juego: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            Admin admin = new Admin(idUsuario, UsuarioActual);
            admin.Show();
        }

        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            usuarioToolStripMenuItem.Text = UsuarioActual;
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            this.Close();

            // Abrir Form3 (Ventana Home)
            Home home = new Home(idUsuario, UsuarioActual, true);
            home.Show();
        }

        private void cerrarSesionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Confirmar cierre de sesión
            DialogResult confirmacion = MessageBox.Show(
                "¿Estás seguro de que deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                this.Close();

                // Reabre Form3
                InicioSesion inicioSesion = new InicioSesion();
                inicioSesion.Show();
            }
        }
    }
}

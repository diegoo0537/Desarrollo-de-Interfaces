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
    public partial class InicioSesion : Form
    {
        public string UsuarioLogueado { get; private set; }

        public InicioSesion()
        {
            InitializeComponent();

            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Recoge los valores ingresados en los TextBox
            string nombre = textBox1.Text.Trim();
            string contraseña = textBox2.Text.Trim();

            // Valida que los campos no estén vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor, rellena todos los campos.");
                return;
            }

            // Validar que el nombre comience con una mayúscula, solo contenga letras y no supere los 20 caracteres
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

            // Define la cadena de conexión
            string datos = "Server=localhost;Port=3306;" +
                           "Database=prueba_desarrollo_interfaces;" +
                           "User ID=root;" +
                           "Password=1234;" +
                           "Pooling=true;";

            // Consulta para verificar si el usuario existe y si está baneado
            string consulta = "SELECT id, nombre, administrador, estado FROM usuarios WHERE nombre = @nombre AND contraseña = @contraseña;";

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(datos))
                {
                    conexion.Open();

                    using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                    {
                        // Añade los parámetros para evitar inyecciones SQL
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@contraseña", contraseña);

                        MySqlDataReader reader = comando.ExecuteReader();

                        if (reader.Read())  // Si encuentra el usuario
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string estado = reader["estado"].ToString();
                            UsuarioLogueado = reader["nombre"].ToString();
                            string esAdministrador = reader["administrador"].ToString();

                            // Verificar si el usuario está baneado
                            if (estado.ToLower() == "baneado")
                            {
                                MessageBox.Show("Tu cuenta está baneada. No puedes iniciar sesión.");
                                return;
                            }

                            // Verificar si el usuario es administrador
                            if (esAdministrador.ToLower() == "si")
                            {
                                // Abrir Form4 (Ventana de Administrador)
                                Admin adminForm = new Admin(id, UsuarioLogueado);
                                adminForm.Show();
                            }
                            else
                            {
                                // Abrir Form3 (Ventana Home)
                                Home home = new Home(id, UsuarioLogueado, false);
                                home.Show();
                            }

                            this.Hide();  // Oculta el Form1
                        }
                        else
                        {
                            // Pregunta si desea registrar al usuario
                            DialogResult msj = MessageBox.Show(
                                "El usuario o contraseña no es correcto. El usuario no esta registrado. ¿Deseas registrarlo?",
                                "Usuario no encontrado",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (msj == DialogResult.Yes)
                            {
                                textBox1.Clear();
                                textBox2.Clear();

                                this.Hide();

                                RegistroSesion registro = new RegistroSesion();
                                registro.Show();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();

            RegistroSesion registro = new RegistroSesion();
            registro.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Si el TextBox tiene configurada la propiedad UseSystemPasswordChar como true (contraseña oculta)
            if (textBox2.UseSystemPasswordChar)
            {
                // Muestra la contraseña
                textBox2.UseSystemPasswordChar = false;
                // Cambiar la imagen del PictureBox a ojo abierto
                pictureBox1.Image = Image.FromFile(@"Resources\open_eye.png");
            }
            else
            {
                // Oculta la contraseña
                textBox2.UseSystemPasswordChar = true;
                // Cambiar la imagen del PictureBox a ojo cerrado
                pictureBox1.Image = Image.FromFile(@"Resources\closed_eye.png");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }
    }
}

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
using MySql.Data.MySqlClient;

namespace Proyecto1
{
    public partial class RegistroSesion : Form
    {
        public RegistroSesion()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Recoge los valores ingresados en los TextBox
            string nombre = textBox1.Text.Trim();
            string contraseña = textBox2.Text.Trim();

            // Obtiene "si" o "no" según el estado del CheckBox
            string esAdministrador = checkBox1.Checked ? "si" : "no";

            // El estado por defecto será "activo"
            string estado = "activo";

            // Valida que los campos no estén vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor, rellena todos los campos.");
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
                MessageBox.Show("La contraseña debe tener entre 4 y 16 caracteres alfanuméricos.");
                return;
            }

            // Define la cadena de conexión
            string datos = "Server=localhost;Port=3306;" +
                           "Database=prueba_desarrollo_interfaces;" +
                           "User ID=root;" +
                           "Password=1234;" +
                           "Pooling=true;";

            // Consultas para verificar e insertar usuarios
            string consultaVerificar = "SELECT COUNT(*) FROM usuarios WHERE contraseña = @contraseña;";
            string consultaInsertar = "INSERT INTO usuarios (nombre, contraseña, administrador, estado) VALUES (@nombre, @contraseña, @administrador, @estado);";

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(datos))
                {
                    conexion.Open();

                    // Verificar si la contraseña ya existe en la base de datos
                    using (MySqlCommand comandoVerificar = new MySqlCommand(consultaVerificar, conexion))
                    {
                        comandoVerificar.Parameters.AddWithValue("@contraseña", contraseña);  // Verificar si la contraseña ya está en uso
                        int contraseñaExistente = Convert.ToInt32(comandoVerificar.ExecuteScalar());

                        if (contraseñaExistente > 0)
                        {
                            MessageBox.Show("Ya existe una cuenta con esta contraseña. Por favor, usa una contraseña diferente.");
                            return;
                        }
                    }

                    // Insertar el nuevo usuario
                    using (MySqlCommand comandoInsertar = new MySqlCommand(consultaInsertar, conexion))
                    {
                        comandoInsertar.Parameters.AddWithValue("@nombre", nombre);
                        comandoInsertar.Parameters.AddWithValue("@contraseña", contraseña);
                        comandoInsertar.Parameters.AddWithValue("@administrador", esAdministrador);
                        comandoInsertar.Parameters.AddWithValue("@estado", estado);

                        int filasAfectadas = comandoInsertar.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Usuario registrado.");

                            this.Close();

                            InicioSesion inicioSesion = new InicioSesion();
                            inicioSesion.Show();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo registrar el usuario.");

                            textBox1.Clear();
                            textBox2.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar el usuario: {ex.Message}");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

            InicioSesion inicioSesion = new InicioSesion();
            inicioSesion.Show();
        }
    }
}

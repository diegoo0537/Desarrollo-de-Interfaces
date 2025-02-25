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
    public partial class EliminarUsuario : Form
    {
        public EliminarUsuario()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Obtener el ID ingresado en el TextBox
            string idUsuario = textBox1.Text.Trim();

            // Define la cadena de conexión
            string datos = "Server=localhost;Port=3306;" +
                           "Database=prueba_desarrollo_interfaces;" +
                           "User ID=root;" +
                           "Password=1234;" +
                           "Pooling=true;";

            if (string.IsNullOrEmpty(idUsuario))
            {
                MessageBox.Show("Por favor, ingresa un ID válido.");
                return;
            }

            // Verificar que el ID solo contenga números
            if (!int.TryParse(idUsuario, out _))
            {
                MessageBox.Show("El ID debe contener solo números.");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(datos))
                {
                    connection.Open();

                    // Verificar si el usuario existe
                    string consultaVerificar = "SELECT COUNT(*) FROM usuarios WHERE id = @id;";
                    using (MySqlCommand comandoVerificar = new MySqlCommand(consultaVerificar, connection))
                    {
                        comandoVerificar.Parameters.AddWithValue("@id", idUsuario);
                        int cantidad = Convert.ToInt32(comandoVerificar.ExecuteScalar());

                        if (cantidad == 0)
                        {
                            MessageBox.Show("El usuario con el ID ingresado no existe.");
                            return;
                        }
                    }

                    // Eliminar compras del usuario
                    string consultaEliminarCompras = "DELETE FROM compras WHERE idUsuario = @id;";
                    using (MySqlCommand comandoEliminarCompras = new MySqlCommand(consultaEliminarCompras, connection))
                    {
                        comandoEliminarCompras.Parameters.AddWithValue("@id", idUsuario);
                        comandoEliminarCompras.ExecuteNonQuery();
                    }

                    // Eliminar al usuario si existe
                    string consultaEliminar = "DELETE FROM usuarios WHERE id = @id;";
                    using (MySqlCommand comandoEliminar = new MySqlCommand(consultaEliminar, connection))
                    {
                        comandoEliminar.Parameters.AddWithValue("@id", idUsuario);
                        comandoEliminar.ExecuteNonQuery();
                    }

                    MessageBox.Show("Usuario eliminado correctamente.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el usuario: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}

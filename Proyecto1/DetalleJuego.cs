using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto1
{
    public partial class DetalleJuego : UserControl
    {
        public int idUsuario { get; set; }
        private Carrito carritoControl = null;

        private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";
        private int juegoId;

        public DetalleJuego(int idUs, int id, Carrito carrito)
        {
            InitializeComponent();

            idUsuario = idUs;
            juegoId = id;
            carritoControl = carrito;

            CargarDatosJuego();
        }

        //cargar los datos del juego
        private void CargarDatosJuego()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "SELECT titulo, precio, descripcion, imagen, categoria FROM juegos WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        command.Parameters.AddWithValue("@id", juegoId);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            label1.Text = reader["titulo"].ToString();
                            label3.Text = reader["precio"].ToString() + "€";
                            label2.Text = reader["descripcion"].ToString();
                            label4.Text = reader["categoria"].ToString();

                            byte[] imgData = reader["imagen"] as byte[];
                            if (imgData != null)
                            {
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    pictureBox1.Image = Image.FromStream(ms);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se encontraron datos del juego.");
                            this.Parent?.Controls.Remove(this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Parent?.Controls.Remove(this);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        //boton añadir
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "INSERT INTO compras (idUsuario, idJuego, adquirido) VALUES (@idUsuario, @idJuego, 'no')";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        command.Parameters.AddWithValue("@idJuego", juegoId);

                        int resultado = command.ExecuteNonQuery();

                        if (resultado > 0)
                        {
                            MessageBox.Show("Juego añadido al carrito.");
                            this.Parent?.Controls.Remove(this);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo añadir el juego.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al añadir el juego al carrito: {ex.Message}");
            }
        }

        //boton eliminar del carrito
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "DELETE FROM compras WHERE idUsuario = @idUsuario AND idJuego = @idJuego AND adquirido = 'no' LIMIT 1";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        command.Parameters.AddWithValue("@idJuego", juegoId);

                        int resultado = command.ExecuteNonQuery();

                        if (resultado > 0)
                        {
                            MessageBox.Show("Juego eliminado del carrito.");

                            carritoControl?.CargarCarrito();

                            this.Parent?.Controls.Remove(this);
                        }
                        else
                        {
                            MessageBox.Show("El juego no esta en el carrito.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el juego del carrito: {ex.Message}");
            }
        }
    }
}

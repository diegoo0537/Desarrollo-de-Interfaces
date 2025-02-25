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
    public partial class Carrito : UserControl
    {
        private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";
        private int idUsuario;

        public Carrito(int idUsuario)
        {
            InitializeComponent();

            this.idUsuario = idUsuario;
            CargarCarrito();
        }

        public void CargarCarrito()
        {
            flowLayoutPanel1.Controls.Clear();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "SELECT compras.idCompra, juegos.id, juegos.titulo, juegos.precio, juegos.imagen FROM compras " +
                                   "JOIN juegos ON compras.idJuego = juegos.id " +
                                   "WHERE compras.idUsuario = @idUsuario AND compras.adquirido = 'no'";


                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string titulo = reader["titulo"].ToString();
                            string precio = reader["precio"].ToString();
                            byte[] imgData = reader["imagen"] as byte[];

                            Image imagen = null;
                            if (imgData != null)
                            {
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    imagen = Image.FromStream(ms);
                                }
                            }

                            // Crear un control de juego para el carrito
                            CatalogoControl juegoEnCarrito = new CatalogoControl(idUsuario, id, titulo, precio + "€", imagen, this);
                            flowLayoutPanel1.Controls.Add(juegoEnCarrito);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el carrito: {ex.Message}");
            }
        }

        private void Carrito_Load(object sender, EventArgs e)
        {

        }

        //boton confirmar compra
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "UPDATE compras SET adquirido = 'si' WHERE idUsuario = @idUsuario AND adquirido = 'no'";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);

                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Compra confirmada. ¡Gracias por tu compra!");
                            CargarCarrito();
                            panel2.Controls.Clear();
                        }
                        else
                        {
                            MessageBox.Show("No hay juegos en el carrito para comprar.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al confirmar la compra: {ex.Message}");
            }
        }

        //boton cancelar
        private void button2_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
        }

        //panel para los detalles de cada juego
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        public void MostrarDetalleJuego(int idJuego)
        {
            // Limpiar el panel antes de agregar el nuevo UserControl
            panel2.Controls.Clear();

            // Crear una instancia de DetalleJuego y agregarla al panel2
            DetalleJuego detalle = new DetalleJuego(idUsuario, idJuego, this);
            detalle.Dock = DockStyle.Fill; // Ajustar al tamaño del panel

            panel2.Controls.Add(detalle);
            panel2.BringToFront(); // Asegurar que está al frente
        }
    }
}

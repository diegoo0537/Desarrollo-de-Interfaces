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
    public partial class MisJuegos : Form
    {
        private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";
        private int idUsuario;
        public string UsuarioActual { get; set; }
        public bool EsAdministrador { get; set; }

        public MisJuegos(int idUsuario, string usuario, bool esAdministrador)
        {
            InitializeComponent();

            this.idUsuario = idUsuario;
            UsuarioActual = usuario;
            EsAdministrador = esAdministrador;

            CargarDatosUsuario();
            flowLayoutPanel1.Resize += (s, e) => AjustarTamanioElementos();
            CargarMisJuegos();
        }

        private void CargarDatosUsuario()
        {
            usuarioToolStripMenuItem.Text = UsuarioActual;
        }

        private void AjustarTamanioElementos()
        {
            int anchoElemento = flowLayoutPanel1.Width / 3 - 15; // Ajustamos el ancho para 3 por fila
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                control.Width = anchoElemento;
            }
        }

        private void CargarMisJuegos()
        {
            flowLayoutPanel1.Controls.Clear();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "SELECT juegos.id, juegos.titulo, juegos.imagen FROM compras " +
                                   "JOIN juegos ON compras.idJuego = juegos.id " +
                                   "WHERE compras.idUsuario = @idUsuario AND compras.adquirido = 'si'";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        command.Parameters.AddWithValue("@idUsuario", idUsuario);
                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            int idJuego = Convert.ToInt32(reader["id"]);
                            string titulo = reader["titulo"].ToString();
                            byte[] imgData = reader["imagen"] as byte[];

                            Image imagen = null;
                            if (imgData != null)
                            {
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    imagen = Image.FromStream(ms);
                                }
                            }

                            // Crear control DetalleMisJuegos y agregarlo al panel
                            DetalleMisJuegos juegoComprado = new DetalleMisJuegos(idUsuario, idJuego, titulo, imagen);
                            juegoComprado.Width = flowLayoutPanel1.Width / 3 - 0; // Ajustamos para que haya 3 por fila
                            
                            flowLayoutPanel1.Controls.Add(juegoComprado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los juegos adquiridos: {ex.Message}");
            }
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
                this.Close(); // Cierra Form1

                // Reabre Form3
                InicioSesion inicioSesion = new InicioSesion();
                inicioSesion.Show();
            }
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            Home home = new Home(idUsuario, UsuarioActual, EsAdministrador);
            home.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Proyecto1
{
    public partial class Home : Form
    {
        public int idUsuario { get; set; }
        public string UsuarioActual { get; set; }
        public bool EsAdministrador { get; set; }

        private Carrito carritoControl = null;

        private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";

        public Home(int id, string usuario, bool esAdministrador)
        {
            InitializeComponent();

            idUsuario = id;
            UsuarioActual = usuario;
            EsAdministrador = esAdministrador;
            
            CargarDatosUsuario();
            CargarCatalogo();
            CargarOfertas();
        }

        private void CargarDatosUsuario()
        {
            usuarioToolStripMenuItem.Text = UsuarioActual;

            adminToolStripMenuItem.Visible = EsAdministrador;
        }

        private void CargarCatalogo(string categoria = null)
        {
            flowLayoutPanel1.Controls.Clear();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "SELECT id, titulo, precio, imagen FROM juegos";

                    if (!string.IsNullOrEmpty(categoria) && categoria != "Otros juegos")
                    {
                        query += " WHERE categoria = @categoria";
                    }

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
                        if (!string.IsNullOrEmpty(categoria) && categoria != "Otros juegos")
                        {
                            command.Parameters.AddWithValue("@categoria", categoria);
                        }

                        MySqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["id"]);
                            string titulo = reader["titulo"].ToString();
                            string precio = reader["precio"].ToString();
                            byte[] imgData = reader["imagen"] as byte[];

                            // Convertir la imagen de bytes a imagen
                            Image imagen = null;
                            if (imgData != null)
                            {
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    imagen = Image.FromStream(ms);
                                }
                            }

                            // Crear un nuevo control de catálogo y agregarlo al FlowLayoutPanel
                            CatalogoControl catalogoControl = new CatalogoControl(idUsuario, id, titulo, precio + "€", imagen, carritoControl);
                            flowLayoutPanel1.Controls.Add(catalogoControl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los juegos: {ex.Message}");
            }
        }

        private void CargarOfertas()
        {
            flowLayoutPanel2.Controls.Clear();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(conexionDB))
                {
                    conexion.Open();
                    string query = "SELECT id, titulo, precio, imagen FROM juegos WHERE precio < 30";

                    using (MySqlCommand command = new MySqlCommand(query, conexion))
                    {
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

                            CatalogoControl catalogoControl = new CatalogoControl(idUsuario, id, titulo, precio + "€", imagen, carritoControl);
                            flowLayoutPanel2.Controls.Add(catalogoControl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ofertas: {ex.Message}");
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void filtrarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void miPerfilToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
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

        private void inicioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            Admin admin = new Admin(idUsuario, UsuarioActual);
            admin.Show();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear(); // Elimina los juegos actuales del catálogo
            CargarCatalogo(); // Recarga el panel
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CargarCatalogo("PS5");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CargarCatalogo("PS4");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CargarCatalogo("XBOX");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CargarCatalogo("PC");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CargarCatalogo("Switch");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CargarCatalogo("Otros juegos");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Clear();
            CargarOfertas();
        }

        //pestaña Carrito
        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (carritoControl == null)
            {
                // Crear el carrito y agregarlo al formulario
                carritoControl = new Carrito(idUsuario);
                carritoControl.Dock = DockStyle.Fill; // Ajustar tamaño
                Controls.Add(carritoControl);
                carritoControl.BringToFront();
            }
            else
            {
                // Si ya está abierto, lo cerramos
                Controls.Remove(carritoControl);
                carritoControl.Dispose();
                carritoControl = null;
            }
        }

        private void flowLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        //pestaña Mis juegos
        private void misJuegosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();

            MisJuegos misJuegos = new MisJuegos(idUsuario, UsuarioActual, EsAdministrador);
            misJuegos.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

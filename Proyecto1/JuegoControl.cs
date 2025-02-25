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

namespace Proyecto1
{
    public partial class JuegoControl : UserControl
    {
        private string conexionString = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";
        private string modoActual = "añadir"; // Puede ser 'añadir', 'editar' o 'eliminar'

        public JuegoControl()
        {
            InitializeComponent();

            CargarCategorias();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void ConfigurarModo(string modo)
        {
            modoActual = modo;
            button2.Text = modo.ToUpper();

            // Limpiar los campos siempre que cambie de modo
            LimpiarCampos();
            CargarCategorias();

            if (modo == "añadir")
            {
                textBox4.ReadOnly = true; // Bloquea el campo ID
                label6.Visible = true;    // Muestra el label6
                button3.Visible = false;  // Oculta el botón de búsqueda
            }
            else if (modo == "editar")
            {
                textBox4.ReadOnly = false; // Permite editar el ID
                label6.Visible = false;    // Oculta label6
                button3.Visible = true;    // Muestra el botón de búsqueda
            }
            else if (modo == "eliminar")
            {
                textBox4.ReadOnly = false; // Permite editar el ID
                label6.Visible = false;    // Oculta label6
                button3.Visible = false;    // Muestra el botón de búsqueda
            }
        }

        //boton seleccionar imagen
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de imagen|*.jpg;*.png;*.jpeg",
                Title = "Seleccionar imagen"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

        //boton guardar
        private void button2_Click(object sender, EventArgs e)
        {
            if (modoActual == "añadir")
                AñadirJuego();
            else if (modoActual == "editar")
                EditarJuego();
            else if (modoActual == "eliminar")
                EliminarJuego();
        }

        //boton buscar juego
        private void button3_Click(object sender, EventArgs e)
        {
            BuscarJuego();
        }

        //boton cancelar
        private void button4_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
        }

        private void AñadirJuego()
        {
            textBox4.ReadOnly = true;
            label6.Visible = true;

            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) || pictureBox1.Image == null || comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Todos los campos excepto ID deben estar completos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(conexionString))
                {
                    conn.Open();
                    string query = "INSERT INTO juegos (titulo, descripcion, precio, imagen, categoria) VALUES (@titulo, @descripcion, @precio, @imagen, @categoria)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@titulo", textBox1.Text);
                        cmd.Parameters.AddWithValue("@descripcion", textBox2.Text);
                        cmd.Parameters.AddWithValue("@precio", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@imagen", ImageToByteArray(pictureBox1.Image));
                        cmd.Parameters.AddWithValue("@categoria", comboBox1.SelectedItem.ToString());

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Juego añadido.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al añadir juego: " + ex.Message);
            }
        }

        private void EditarJuego()
        {
            textBox4.ReadOnly = false;
            label6.Visible = false;

            if (string.IsNullOrWhiteSpace(textBox4.Text) || comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Debes ingresar un ID y seleccionar una categoría.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text, out int idText))
            {
                MessageBox.Show("Debes ingresar un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(conexionString))
                {
                    conn.Open();
                    string query = "UPDATE juegos SET titulo=@titulo, descripcion=@descripcion, precio=@precio, imagen=@imagen, categoria=@categoria WHERE id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idText);
                        cmd.Parameters.AddWithValue("@titulo", textBox1.Text);
                        cmd.Parameters.AddWithValue("@descripcion", textBox2.Text);
                        cmd.Parameters.AddWithValue("@precio", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@imagen", ImageToByteArray(pictureBox1.Image));
                        cmd.Parameters.AddWithValue("@categoria", comboBox1.SelectedItem.ToString());

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Juego actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el juego.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar juego: " + ex.Message);
            }
        }

        private void EliminarJuego()
        {
            textBox4.ReadOnly = false;
            label6.Visible = false;

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Debes ingresar un ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text, out int idText))
            {
                MessageBox.Show("Debes ingresar un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(conexionString))
                {
                    conn.Open();
                    string query = "DELETE FROM juegos WHERE id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idText);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        MessageBox.Show(filasAfectadas > 0 ? "Juego eliminado." : "No se encontró el juego.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar juego: " + ex.Message);
            }
        }

        private void BuscarJuego()
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Debes ingresar un ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text, out int idText))
            {
                MessageBox.Show("Debes ingresar un ID válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(conexionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM juegos WHERE id=@id LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idText);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBox1.Text = reader["titulo"].ToString();
                                textBox2.Text = reader["descripcion"].ToString();
                                textBox3.Text = reader["precio"].ToString();
                                comboBox1.SelectedItem = reader["categoria"].ToString();

                                if (reader["imagen"] != DBNull.Value)
                                    pictureBox1.Image = ByteArrayToImage((byte[])reader["imagen"]);
                                else
                                    pictureBox1.Image = null;
                            }
                            else
                            {
                                MessageBox.Show("No se encontró el juego.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar juego: " + ex.Message);
            }
        }

        private void CargarCategorias()
        {
            comboBox1.Items.Clear();

            comboBox1.Items.Add("PS5");
            comboBox1.Items.Add("PS4");
            comboBox1.Items.Add("XBOX");
            comboBox1.Items.Add("PC");
            comboBox1.Items.Add("Switch");
            comboBox1.Items.Add("Otros juegos");
        }

        private void LimpiarCampos()
        {
            textBox4.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            pictureBox1.Image = null;
            comboBox1.Items.Clear();
        }

        private byte[] ImageToByteArray(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
    }
}

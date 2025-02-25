using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto1
{
    public partial class DetalleMisJuegos : UserControl
    {
        private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";
        private int idUsuario;
        private int idJuego;

        public DetalleMisJuegos(int idUsuario, int idJuego, string titulo, Image imagen)
        {
            InitializeComponent();

            this.idUsuario = idUsuario;
            this.idJuego = idJuego;
            label1.Text = titulo;
            pictureBox1.Image = imagen;
        }

        //boton jugar
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}

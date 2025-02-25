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
    public partial class CatalogoControl : UserControl
    {
        private DetalleJuego detalleJuegoActual = null;
        private bool detalleAbierto = false; // Bandera para verificar si ya hay un detalle abierto

        public int idUsuario { get; set; }

        private int idJuego;

        private Carrito carritoControl;

        //private string conexionDB = "Server=localhost;Port=3306;Database=prueba_desarrollo_interfaces;User ID=root;Password=1234;Pooling=true;";

        public CatalogoControl(int idUs, int id, string titulo, string precio, Image imagen, Carrito carritoControl)
        {
            InitializeComponent();

            idUsuario = idUs;
            idJuego = id;
            label1.Text = titulo;
            label2.Text = precio;
            pictureBox1.Image = imagen;
            this.carritoControl = carritoControl;

            //this.Click += new EventHandler(AbrirDetalle);
            this.Click += new EventHandler(AbrirDetalle);
            pictureBox1.Click += new EventHandler(AbrirDetalle);
            label1.Click += new EventHandler(AbrirDetalle);
            label2.Click += new EventHandler(AbrirDetalle);
        }

        private void AbrirDetalle(object sender, EventArgs e)
        {
            if (carritoControl != null)
            {
                // Si el control está dentro del carrito, lo abre en el panel2
                carritoControl.MostrarDetalleJuego(idJuego);
            }
            else
            {
                Form homeForm = this.FindForm(); // Encuentra el formulario principal

                if (homeForm != null)
                {
                    // Si ya hay un detalle abierto, lo cerramos
                    if (detalleAbierto)
                    {
                        CerrarDetalle();
                    }

                    // Crear una nueva instancia de DetalleJuego
                    detalleJuegoActual = new DetalleJuego(idUsuario, idJuego, carritoControl);

                    detalleJuegoActual.Top = 30;

                    // Agregar el nuevo UserControl dentro del formulario
                    homeForm.Controls.Add(detalleJuegoActual);
                    detalleJuegoActual.BringToFront(); // Asegura que el nuevo control esté al frente

                    // Establecer la bandera para indicar que el detalle está abierto
                    detalleAbierto = true;
                }
                else
                {
                    MessageBox.Show("No se encontró el formulario principal.");
                }
            }
        }

        public void CerrarDetalle()
        {
            if (detalleJuegoActual != null)
            {
                Form homeForm = this.FindForm();

                homeForm.Parent?.Controls.Remove(detalleJuegoActual);

                if (homeForm != null)
                {
                    homeForm.Controls.Remove(detalleJuegoActual);
                    detalleJuegoActual.Dispose();
                    detalleJuegoActual = null;
                    detalleAbierto = false; // Restablecer la bandera al cerrar el detalle
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionContenedores
{
    public partial class CambioEstado : Form
    {
        private Contenedores contenedorActual; // El objeto LINQ
        public CambioEstado(Contenedores contenedor)
        {
            InitializeComponent();
            this.contenedorActual = contenedor;
            CargarDatosIniciales();
        }

        public void PrellenarCoordenadas(double lat, double lng)
        {
            txtLatitud.Text = lat.ToString();
            txtLongitud.Text = lng.ToString();
        }
        private void CargarDatosIniciales()
        {
            // Llenar combo
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Util");
            cmbEstado.Items.Add("Lleno");

            if (contenedorActual != null)
            {
                // MODO EDICIÓN
                this.Text = "Editar Contenedor";
                txtId.Text = contenedorActual.Id.ToString();
                txtNombre.Text = contenedorActual.Nombre;
                txtDireccion.Text = contenedorActual.Direccion;
                txtLatitud.Text = contenedorActual.Latitud.ToString();
                txtLongitud.Text = contenedorActual.Longitud.ToString();
                cmbEstado.SelectedItem = contenedorActual.Estado;

                // Ocultar botón agregar, mostrar guardar cambios
                btnAgregar.Visible = false;
                btnGuardar.Visible = true; // Este será "Guardar Cambios"

                // Ajustar radio buttons
                if (contenedorActual.Estado == "Lleno") rbtnLleno.Checked = true;
                else rbtnUtilizable.Checked = true;
            }
            else
            {
                // MODO NUEVO
                this.Text = "Nuevo Contenedor";
                btnAgregar.Visible = true; // Este crea el nuevo
                btnGuardar.Visible = false;
                cmbEstado.SelectedIndex = 0;
            }
        }
        private void ConfigurarFormularioAgregar()
        {
            
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Util");
            cmbEstado.Items.Add("Lleno");
            cmbEstado.SelectedIndex = 0; 

            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtId.Text = "";
            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtLatitud.Text = "";
            txtLongitud.Text = "";
            cmbEstado.SelectedIndex = 0;
        }

        private void CargarComboBox()
        {
            
        }

        private void cmbContenedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if (cmbContenedores.SelectedIndex >= 0)
            {
                int contenedorId = int.Parse(cmbContenedores.SelectedItem.ToString().Split('-')[0].Trim());
                contenedorSeleccionado = contenedores.FirstOrDefault(c => c.Id == contenedorId);

                if (contenedorSeleccionado != null)
                {
                    MostrarDatosContenedor();
                }
            }
            */
        }

        /*
        private void MostrarDatosContenedor()
        {
            

            // Establecer el radio button correspondiente al estado actual
            if (contenedorSeleccionado.Estado == "Util")
            {
                rbtnUtilizable.Checked = true;
            }
            else
            {
                rbtnLleno.Checked = true;
            }
        }
        */

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new GestionDBDataContext())
                {
                    // Buscamos el registro fresco en la BD
                    var c = db.Contenedores.SingleOrDefault(x => x.Id == contenedorActual.Id);

                    if (c != null)
                    {
                        // Actualizamos estado (o lo que se haya editado)
                        c.Estado = rbtnLleno.Checked ? "Lleno" : "Util";

                        db.SubmitChanges();
                        MessageBox.Show("Estado actualizado.");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new GestionDBDataContext())
                {
                    Contenedores nuevo = new Contenedores
                    {
                        Nombre = txtNombre.Text,
                        Direccion = txtDireccion.Text,
                        Latitud = double.Parse(txtLatitud.Text),
                        Longitud = double.Parse(txtLongitud.Text),
                        Estado = cmbEstado.Text
                    };

                    db.Contenedores.InsertOnSubmit(nuevo);
                    db.SubmitChanges();

                    MessageBox.Show("Contenedor agregado correctamente.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}

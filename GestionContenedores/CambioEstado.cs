using GestionContenedores.Models;
using GestionContenedores.Services;
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
        private Contenedor contenedorActual; // El objeto LINQ
        public CambioEstado(Contenedor contenedor)
        {
            InitializeComponent();
            contenedorActual = contenedor;

            if (contenedorActual != null)
            {
                // Es EDICIÓN: Cargar datos en los textbox
                txtId.Text = contenedorActual.Id.ToString();
                txtNombre.Text = contenedorActual.Nombre;
                // ... cargar resto de campos ...

                // Configurar UI
                btnAgregar.Enabled = false; // Ocultar botón agregar
                btnGuardar.Enabled = true;  // Mostrar botón guardar cambios
            }
            else
            {
                // Es NUEVO
                btnAgregar.Enabled = true;
                btnGuardar.Enabled = false;
            }
        }

        public void PrellenarCoordenadas(double lat, double lng)
        {
            txtLatitud.Text = lat.ToString();
            txtLongitud.Text = lng.ToString();
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
            cmbContenedores.Items.Clear();

            foreach (var contenedor in contenedores)
            {
                cmbContenedores.Items.Add($"{contenedor.Id} - {contenedor.Nombre}");
            }

            if (cmbContenedores.Items.Count > 0)
            {
                cmbContenedores.SelectedIndex = 0;
            }
        }

        private void cmbContenedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbContenedores.SelectedIndex >= 0)
            {
                int contenedorId = int.Parse(cmbContenedores.SelectedItem.ToString().Split('-')[0].Trim());
                contenedorSeleccionado = contenedores.FirstOrDefault(c => c.Id == contenedorId);

                if (contenedorSeleccionado != null)
                {
                    MostrarDatosContenedor();
                }
            }
        }

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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (var db = new GestionDBDataContext())
            {
                // Volvemos a buscar el objeto en este contexto para editarlo
                var c = db.Contenedores.Single(x => x.Id == contenedorActual.Id);

                c.Estado = rbtnUtilizable.Checked ? "Util" : "Lleno";
                // Actualizar otros campos si es necesario...

                db.SubmitChanges(); // LINQ detecta el cambio y hace el UPDATE solo

                MessageBox.Show("Estado actualizado!");
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            using (var db = new GestionDBDataContext())
            {
                Contenedor nuevo = new Contenedor
                {
                    Nombre = txtNombre.Text,
                    Direccion = txtDireccion.Text,
                    Latitud = double.Parse(txtLatitud.Text),
                    Longitud = double.Parse(txtLongitud.Text),
                    Estado = cmbEstado.Text
                };

                db.Contenedores.InsertOnSubmit(nuevo); // Prepara la inserción
                db.SubmitChanges(); // Ejecuta el SQL

                MessageBox.Show("Guardado!");
                this.Close();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}

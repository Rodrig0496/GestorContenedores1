using GestionContenedores.Models;
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
        private List<Contenedor> contenedores;
        private Contenedor contenedorSeleccionado;
        public event EventHandler EstadoCambiado;
        public CambioEstado(List<Contenedor> contenedoresList)
        {
            InitializeComponent();
            contenedores = contenedoresList;
            CargarComboBox();
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
            if (contenedorSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un contenedor", "Advertencia",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Actualizar estado según el radio button seleccionado
            contenedorSeleccionado.Estado = rbtnUtilizable.Checked ? "Util" : "Lleno";

            // Disparar evento para notificar el cambio
            EstadoCambiado?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

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
        private List<Contenedor> contenedores;
        private Contenedor contenedorSeleccionado;
        public event EventHandler EstadoCambiado;
        public event EventHandler ContenedorAgregado;
        public CambioEstado(List<Contenedor> contenedoresList)
        {
            InitializeComponent();
            contenedores = contenedoresList;
            CargarComboBox();
            ConfigurarFormularioAgregar();
        }

        public void PrellenarCoordenadas(double latitud, double longitud)
        {
            // Asignamos los valores a los TextBox
            txtLatitud.Text = latitud.ToString();
            txtLongitud.Text = longitud.ToString();

            // Opcional: Seleccionar automáticamente el estado "Util" para un nuevo contenedor
            cmbEstado.SelectedIndex = 0;
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

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos
                if (string.IsNullOrWhiteSpace(txtId.Text) ||
                    string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                    string.IsNullOrWhiteSpace(txtLatitud.Text) ||
                    string.IsNullOrWhiteSpace(txtLongitud.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar que el ID no exista
                int nuevoId = int.Parse(txtId.Text);
                if (contenedores.Any(c => c.Id == nuevoId))
                {
                    MessageBox.Show("El ID ya existe. Por favor, use un ID diferente", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Crear nuevo contenedor
                Contenedor nuevoContenedor = new Contenedor
                {
                    Id = nuevoId,
                    Nombre = txtNombre.Text,
                    Direccion = txtDireccion.Text,
                    Latitud = double.Parse(txtLatitud.Text),
                    Longitud = double.Parse(txtLongitud.Text),
                    Estado = cmbEstado.SelectedItem.ToString()
                };

                // Agregar a la lista local
                contenedores.Add(nuevoContenedor);

                // Guardar en archivo
                FileService.GuardarContenedores(contenedores);

                // Disparar evento
                ContenedorAgregado?.Invoke(this, EventArgs.Empty);

                // Actualizar combobox
                CargarComboBox();

                // Limpiar campos
                LimpiarCampos();

                MessageBox.Show("Contenedor agregado correctamente", "Éxito",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Por favor, ingrese valores válidos para ID, Latitud y Longitud", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar contenedor: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}

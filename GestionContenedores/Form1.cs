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
using System.Windows.Forms.DataVisualization.Charting;

namespace GestionContenedores
{
    public partial class Form1 : Form
    {
        private List<Contenedor> contenedores;
        private int nivelPermiso;
        private string usuarioActual;

        public Form1(int nivelPermiso, string usuario)
        {
            InitializeComponent();
            this.nivelPermiso = nivelPermiso;
            this.usuarioActual = usuario;

            ConfigurarGrafico();
            CargarDatos();

            
            if (nivelPermiso == 1) 
            {
                btnCambiarEstado.Enabled = false;
                MessageBox.Show($"Bienvenido {usuario} (solo lectura)", "Acceso limitado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Bienvenido {usuario} (admin)", "Acceso total",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void ConfigurarGrafico()
        {
            
            chartBarras.Titles.Clear();
            chartBarras.Series.Clear();
            chartBarras.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chartBarras.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            
            chartBarras.Legends[0].Enabled = false;

            
            chartBarras.Titles.Add("CONTENEDORES POR ESTADO");
            chartBarras.Titles[0].Font = new System.Drawing.Font("Arial", 12, FontStyle.Bold);
        }

        private void ActualizarGrafico()
        {
            
            chartBarras.Series.Clear();

           
            Series series = new Series("EstadosContenedores");
            series.ChartType = SeriesChartType.Column;
            series.IsValueShownAsLabel = true;

            // Contar contenedores por estado
            int utilizables = contenedores.Count(c => c.Estado == "Util");
            int llenos = contenedores.Count(c => c.Estado == "Lleno");

            // Agregar datos al gráfico
            series.Points.AddXY("Utilizables", utilizables);
            series.Points.AddXY("Llenos", llenos);

            // Colores personalizados
            series.Points[0].Color = System.Drawing.Color.LightGreen;
            series.Points[1].Color = System.Drawing.Color.LightCoral;

            // Agregar la serie al gráfico
            chartBarras.Series.Add(series);

            // Actualizar estadísticas
            ActualizarEstadisticas(utilizables, llenos);
        }

        private void ActualizarEstadisticas(int utilizables, int llenos)
        {
            lblEstadisticas.Text = $"Utilizables: {utilizables} | Llenos: {llenos} | Total: {contenedores.Count}";
        }
        private void CargarDatos()
        {
            contenedores = FileService.LeerContenedores();
            ActualizarDataGridView();
            ActualizarGrafico();
        }

        private void ActualizarDataGridView()
        {
            dgvContenedores.DataSource = null;
            dgvContenedores.DataSource = contenedores;

            // Formatear columnas
            dgvContenedores.Columns["Id"].HeaderText = "ID";
            dgvContenedores.Columns["Nombre"].HeaderText = "Nombre";
            dgvContenedores.Columns["Direccion"].HeaderText = "Dirección";
            dgvContenedores.Columns["Latitud"].HeaderText = "Latitud";
            dgvContenedores.Columns["Longitud"].HeaderText = "Longitud";
            dgvContenedores.Columns["Estado"].HeaderText = "Estado";

            // Ajustar tamaño de columnas
            dgvContenedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            var formCambioEstado = new CambioEstado(contenedores);
            formCambioEstado.EstadoCambiado += FormCambioEstado_EstadoCambiado;
            formCambioEstado.ContenedorAgregado += FormCambioEstado_ContenedorAgregado;
            formCambioEstado.ShowDialog();
        }

        private void FormCambioEstado_EstadoCambiado(object sender, EventArgs e)
        {
            // Guardar cambios en el archivo y actualizar DataGridView
            FileService.GuardarContenedores(contenedores);
            ActualizarDataGridView();
            ActualizarGrafico();
            MessageBox.Show("Estado actualizado correctamente", "Éxito",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormCambioEstado_ContenedorAgregado(object sender, EventArgs e)
        {
            // Recargar datos desde el archivo
            CargarDatos();
            MessageBox.Show("Contenedor agregado correctamente", "Éxito",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}

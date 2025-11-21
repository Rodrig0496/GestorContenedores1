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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace GestionContenedores
{
    public partial class Form1 : Form
    {
        private List<Contenedor> contenedores;
        private int nivelPermiso;
        private string usuarioActual;
        private GMapOverlay marcadoresOverlay = new GMapOverlay("marcadores");

        public Form1(int nivelPermiso, string usuario)
        {
            InitializeComponent();
            this.nivelPermiso = nivelPermiso;
            this.usuarioActual = usuario;

            //ConfigurarGrafico();
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

            ConfigurarMapa();

        }

        /*
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
        */

        private void ConfigurarMapa()
        {
            // Configuración básica
            gMapControl1.MapProvider = GMapProviders.GoogleMap; // O OpenStreetMapProvider
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.ShowCenter = false; // Quitar la cruz roja del centro

            // Posición inicial (Tacna, según tus datos)
            gMapControl1.Position = new PointLatLng(-18.0146, -70.2536);
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 13;

            // IMPORTANTE: Agregar la capa al mapa una sola vez
            gMapControl1.Overlays.Add(marcadoresOverlay);
        }

        private void ActualizarMapa()
        {
            // Limpiar marcadores anteriores para no duplicar
            marcadoresOverlay.Markers.Clear();

            if (contenedores == null) return;

            foreach (var item in contenedores)
            {
                // Validación básica de coordenadas (para evitar errores si son 0)
                if (item.Latitud != 0 && item.Longitud != 0)
                {
                    PointLatLng punto = new PointLatLng(item.Latitud, item.Longitud);

                    // Lógica de colores según estado (Lleno = Rojo, Util = Verde)
                    GMarkerGoogleType tipoPin = GMarkerGoogleType.green;

                    if (item.Estado == "Lleno")
                    {
                        tipoPin = GMarkerGoogleType.red;
                    }
                    else if (item.Estado == "Mitad") // Por si agregas este estado luego
                    {
                        tipoPin = GMarkerGoogleType.yellow;
                    }

                    // Crear el marcador
                    GMapMarker marcador = new GMarkerGoogle(punto, tipoPin);

                    // Agregar tooltip (texto al pasar el mouse)
                    marcador.ToolTipText = $"ID: {item.Id}\n{item.Nombre}\nEstado: {item.Estado}";
                    marcador.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                    // Agregarlo al mapa
                    marcadoresOverlay.Markers.Add(marcador);
                }
            }

            // Opcional: Centrar el mapa en el primer contenedor si existe
            if (contenedores.Count > 0)
            {
                gMapControl1.Position = new PointLatLng(contenedores[0].Latitud, contenedores[0].Longitud);
            }

            gMapControl1.Refresh(); // Forzar repintado
        }
        private void ActualizarEstadisticas(int utilizables, int llenos)
        {
            lblEstadisticas.Text = $"Utilizables: {utilizables} | Llenos: {llenos} | Total: {contenedores.Count}";
        }
        private void CargarDatos()
        {
            contenedores = FileService.LeerContenedores();
            ActualizarDataGridView();
            //ActualizarGrafico();
            ActualizarMapa();
            DibujarPinesEnMapa();
        }

        private void DibujarPinesEnMapa()
        {
            // Limpiamos marcadores viejos para no duplicar
            marcadoresOverlay.Markers.Clear();

            if (contenedores == null) return;

            foreach (var item in contenedores)
            {
                // Verificar que las coordenadas sean válidas (diferentes de 0)
                if (item.Latitud != 0 && item.Longitud != 0)
                {
                    PointLatLng punto = new PointLatLng(item.Latitud, item.Longitud);

                    // Lógica de Colores: Rojo si está lleno, Verde si es útil
                    GMarkerGoogleType tipoPin = GMarkerGoogleType.green;
                    if (item.Estado.Trim().Equals("Lleno", StringComparison.OrdinalIgnoreCase))
                    {
                        tipoPin = GMarkerGoogleType.red;
                    }

                    // Crear el marcador
                    GMapMarker marcador = new GMarkerGoogle(punto, tipoPin);

                    // Tooltip: Lo que sale al poner el mouse encima
                    marcador.ToolTipText = $"{item.Nombre}\nEstado: {item.Estado}\n{item.Direccion}";
                    marcador.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                    // Agregar a la capa
                    marcadoresOverlay.Markers.Add(marcador);
                }
            }

            // Refrescar el mapa para ver los cambios
            gMapControl1.Refresh();
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
            //ActualizarGrafico();
            DibujarPinesEnMapa();
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

        private void chartBarras_Click(object sender, EventArgs e)
        {

        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            // 1. Verificar si fue clic DERECHO
            if (e.Button == MouseButtons.Right)
            {
                // 2. Verificar si es ADMINISTRADOR (Nivel 0)
                if (this.nivelPermiso == 0)
                {
                    // 3. Obtener la coordenada geográfica exacta donde se hizo clic
                    // 'FromLocalToLatLng' convierte la posición X,Y del mouse a Lat/Lng real
                    PointLatLng puntoClic = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                    // 4. Preguntar si desea agregar
                    DialogResult respuesta = MessageBox.Show(
                        "¿Quiere ingresar un nuevo contenedor en este punto?",
                        "Nuevo Contenedor",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (respuesta == DialogResult.Yes)
                    {
                        // 5. Instanciar el formulario CambioEstado
                        // Pasamos la lista actual para mantener la referencia
                        var formNuevo = new CambioEstado(this.contenedores);

                        // 6. Pasar las coordenadas al formulario
                        formNuevo.PrellenarCoordenadas(puntoClic.Lat, puntoClic.Lng);

                        // 7. Suscribirnos al evento para recargar si se guarda
                        formNuevo.ContenedorAgregado += FormCambioEstado_ContenedorAgregado;

                        // 8. Mostrar el formulario
                        formNuevo.ShowDialog();
                    }
                }
            }
        }
    }
}

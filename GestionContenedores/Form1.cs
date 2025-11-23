
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
        GestionDBDataContext db = new GestionDBDataContext();

        private List<Contenedores> contenedores;
        private int nivelPermiso;
        private string usuarioActual;
        private GMapOverlay marcadoresOverlay = new GMapOverlay("marcadores");

        public Form1(int nivel, string usuario)
        {
            InitializeComponent();
            this.nivelPermiso = nivel;
            this.usuarioActual = usuario;
            ConfigurarMapa();
            ConfigurarPermisosIniciales();
            CargarDatos();

        }

        private void ConfigurarPermisosIniciales()
        {
            this.Text = $"Gestión de Contenedores - {usuarioActual}";

            // Si es invitado (1), deshabilitamos edición
            bool esAdmin = (nivelPermiso == 0);
            btnCambiarEstado.Visible = esAdmin;

            // Texto del botón según estado
            btnLogin.Text = esAdmin ? "Cerrar Sesión" : "Iniciar Sesión";
        }

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
            db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Contenedores);
            contenedores = db.Contenedores.ToList();
            ActualizarDataGridView();
            ActualizarMapa();
            DibujarPinesEnMapa();
        }

        private void DibujarPinesEnMapa()
        {
            // Limpiamos marcadores viejos para no duplicar
            marcadoresOverlay.Markers.Clear();

            foreach (var item in contenedores)
            {
                // OJO: La clase de LINQ usa 'double?' (nullable) a veces.
                // Aseguramos que tenga valor con .GetValueOrDefault() o casting.
                double lat = item.Latitud;
                double lng = item.Longitud;

                if (lat != 0 && lng != 0)
                {
                    PointLatLng punto = new PointLatLng(lat, lng);

                    GMarkerGoogleType tipo = GMarkerGoogleType.green;
                    if (item.Estado == "Lleno") tipo = GMarkerGoogleType.red;
                    else if (item.Estado == "Mitad") tipo = GMarkerGoogleType.yellow;

                    GMapMarker marcador = new GMarkerGoogle(punto, tipo);
                    marcador.ToolTipText = $"{item.Nombre}\nEstado: {item.Estado}";

                    // Guardamos el ID del contenedor en el Tag del marcador para usarlo luego
                    marcador.Tag = item.Id;

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
            if (e.Button == MouseButtons.Right && nivelPermiso == 0) // Solo admin
            {
                PointLatLng p = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                if (MessageBox.Show("¿Agregar contenedor aquí?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Pasamos 'null' porque es nuevo, no estamos editando uno existente
                    CambioEstado frm = new CambioEstado(null);
                    frm.PrellenarCoordenadas(p.Lat, p.Lng);

                    // Suscribimos al evento para recargar al cerrar
                    frm.FormClosed += (s, args) => CargarDatos();

                    frm.ShowDialog();
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (nivelPermiso == 0) // Si ya es admin y quiere salir
            {
                nivelPermiso = 1;
                usuarioActual = "Invitado";
                ConfigurarPermisosIniciales();
                MessageBox.Show("Sesión cerrada.");
            }
            else // Si es invitado y quiere entrar
            {
                Login frmLogin = new Login();
                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    this.nivelPermiso = frmLogin.NivelPermiso;
                    this.usuarioActual = frmLogin.UsuarioActual;
                    ConfigurarPermisosIniciales();
                    MessageBox.Show($"Bienvenido {usuarioActual}");
                }
            }
        }
    }
}

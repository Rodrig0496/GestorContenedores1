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

namespace GestionContenedores
{
    public partial class Login : Form
    {
        string usersFilePath;

        public int NivelPermiso { get; private set; } = -1; 
        public string UsuarioActual { get; private set; } = "";

        public Login()
        {
            InitializeComponent();
            usersFilePath = Path.Combine(Application.StartupPath, "users.txt");
            CrearArchivoUsuarios();
        }

        private void CrearArchivoUsuarios()
        {
            if (!File.Exists(usersFilePath))
            {
                string[] ejemplo = new string[]
                {
                    "admin;admin123;0", // admin
                    "vecino1;clave1;1"  // usuario
                };
                File.WriteAllLines(usersFilePath, ejemplo);
            }
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContraseña.Text.Trim();

            if (usuario == "" || contrasena == "")
            {
                MessageBox.Show("Por favor ingresa usuario y contraseña.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var lineas = File.ReadAllLines(usersFilePath);

                foreach (var linea in lineas)
                {
                    var partes = linea.Split(';');
                    if (partes.Length == 3 &&
                        partes[0].Trim() == usuario &&
                        partes[1].Trim() == contrasena)
                    {
                        UsuarioActual = usuario;
                        NivelPermiso = int.Parse(partes[2].Trim());
                        this.DialogResult = DialogResult.OK;
                        return;
                    }
                }

                MessageBox.Show("Usuario o contraseña incorrectos.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error leyendo users.txt:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }

        private void lblContraseña_Click(object sender, EventArgs e)
        {

        }

        private void lblUsuario_Click(object sender, EventArgs e)
        {

        }
    }
}

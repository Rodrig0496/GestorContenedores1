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
     

        public int NivelPermiso { get; private set; }
        public string UsuarioActual { get; private set; }

        public Login()
        {
            InitializeComponent();
            
        }


        private void btnIngresar_Click(object sender, EventArgs e)
        {
            using (var db = new GestionDBDataContext())
            {
                // CONSULTA LINQ: Buscar usuario que coincida
                var user = db.Usuarios
                             .FirstOrDefault(u => u.Username == txtUsuario.Text && u.Password == txtContraseña.Text);

                if (user != null)
                {
                    NivelPermiso = user.NivelPermiso;
                    UsuarioActual = user.Username;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Credenciales incorrectas");
                }
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

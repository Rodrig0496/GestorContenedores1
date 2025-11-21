using GestionContenedores.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionContenedores.Services
{
    public static class FileService
    {
        private static string filePath = Path.Combine(Application.StartupPath, "Contenedores.txt");

        public static List<Contenedor> LeerContenedores()
        {
            var contenedores = new List<Contenedor>();

            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Archivo no encontrado: {filePath}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return contenedores;
                }

                var lineas = File.ReadAllLines(filePath);

                foreach (var linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    var datos = linea.Split(';');
                    if (datos.Length >= 6)
                    {
                        // 1. Leemos el número "crudo" del archivo
                        double latTemp = double.Parse(datos[3]);
                        double lonTemp = double.Parse(datos[4]);

                        // 2. LOGICA INTELIGENTE:
                        // Si el número es mayor a 1000 (o menor a -1000), es formato antiguo -> Dividimos.
                        // Si es un número pequeño (como -18), es formato nuevo -> Lo dejamos igual.
                        if (Math.Abs(latTemp) > 1000) latTemp = latTemp / 1000000.0;
                        if (Math.Abs(lonTemp) > 1000) lonTemp = lonTemp / 1000000.0;

                        var contenedor = new Contenedor
                        {
                            Id = int.Parse(datos[0]),
                            Nombre = datos[1],
                            Direccion = datos[2],

                            Latitud = latTemp,
                            Longitud = lonTemp,
                            Estado = datos[5]
                        };
                        contenedores.Add(contenedor);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return contenedores;
        }

        public static void GuardarContenedores(List<Contenedor> contenedores)
        {
            try
            {
                var lineas = new List<string>();

                foreach (var contenedor in contenedores)
                {
                    string linea = $"{contenedor.Id};{contenedor.Nombre};{contenedor.Direccion};" +
                                 $"{contenedor.Latitud};{contenedor.Longitud};{contenedor.Estado}";
                    lineas.Add(linea);
                }

                File.WriteAllLines(filePath, lineas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void AgregarContenedor(Contenedor nuevoContenedor)
        {
            try
            {
                // Leer contenedores existentes
                var contenedores = LeerContenedores();

                // Agregar el nuevo contenedor
                contenedores.Add(nuevoContenedor);

                // Guardar todos los contenedores
                GuardarContenedores(contenedores);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar contenedor: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

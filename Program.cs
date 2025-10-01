using Proyecto; //se importan namespaces, que son carpetas con metodos y clases
using ProyectoVentas;

namespace PROYECTO1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); //aranca la app

           
            using (var login = new InicioSesion()) //Abre el formulario de Login al principio
			{
                var result = login.ShowDialog(); //no se puede interactuar con otras forms hasta que se cierre login

                
                if (result != DialogResult.OK || SesionActual.Usuario == null) //la aplicación se cierra si el usuario cancela el login o no se establece un usuario válido
					return;

                
                Application.Run(new FBienvenida()); //Si el usuario es valido, se lanza el formulario principal FBienvenida.
			}
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO1
{
	// UsuarioSesion: clase que representa los datos del usuario que inició sesión
	public class UsuarioSesion 
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int IdRol { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO1
{
	// SesionActual: clase estática que guarda el usuario logueado
	// para que cualquier formulario pueda acceder a su información
	public static class SesionActual 
	{
        public static UsuarioSesion Usuario { get; set; } 

	}
}

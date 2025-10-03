using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PROYECTO1
{
    internal class BaseDatos //clase para manejar la conexión a la base de datos
    {
        private const string CS = @"Server=coumputadora;Database=tiendaDB;Trusted_Connection=True;"; //cadena de conexión a la base de datos

        public static SqlConnection GetConnection() => new SqlConnection(CS); //método que devuelve una nueva conexión a la base de datos
    }
}

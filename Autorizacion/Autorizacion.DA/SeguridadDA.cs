using Autorizacion.Abstracciones.DA;
using Autorizacion.Abstracciones.Modelos;
using Microsoft.Data.SqlClient;
using Dapper;
using Helpers;

namespace Autorizacion.DA
{
    public class SeguridadDA : ISeguridadDA
    {
        IRepositorioDapper _repositorioDapper;
        private SqlConnection _SqlConnection;

        public SeguridadDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
            _SqlConnection = repositorioDapper.ObtenerRepositorioDapper();
        }

        public async Task<IEnumerable<Perfiles>> ObtenerPerfilesUsuario(Usuario usuario)
        {
            string sql = @"[ObtenerPerfilesxUsuario]";
            var consulta= await _SqlConnection.QueryAsync<Abstracciones.Entidades.Perfiles>(sql, new { CorreoElectronico=usuario.CorreoElectronico ,NombreUsuario=usuario.NombreUsuario });
            return Convertidor.ConvertirLista<Abstracciones.Entidades.Perfiles, Abstracciones.Modelos.Perfiles>(consulta);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            string sql = @"[ObtenerUsuario]";
            var consulta = await _SqlConnection.QueryAsync<Abstracciones.Entidades.Usuario>(sql, new { CorreoElectronico = usuario.CorreoElectronico
                , NombreUsuario = usuario.NombreUsuario });
            return Convertidor.Convertir<Abstracciones.Entidades.Usuario, Abstracciones.Modelos.Usuario>(consulta.FirstOrDefault());
        }
    }
}

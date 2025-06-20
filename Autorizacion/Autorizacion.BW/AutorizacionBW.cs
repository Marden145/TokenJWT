using Autorizacion.Abstracciones.BW;
using Autorizacion.Abstracciones.DA;
using Autorizacion.Abstracciones.Modelos;

namespace Autorizacion.BW
{
    public class AutorizacionBW : IAutorizacionBW
    {
        private ISeguridadDA _seguridadDA;

        public AutorizacionBW(ISeguridadDA seguridadDA)
        {
            _seguridadDA = seguridadDA;
        }

        public async Task<IEnumerable<Perfiles>> ObtenerPerfilesUsuario(Usuario usuario)
        {
            return await _seguridadDA.ObtenerPerfilesUsuario(usuario);
        }

        public async Task<Usuario> ObtenerUsuario(Usuario usuario)
        {
            return await _seguridadDA.ObtenerUsuario(usuario);
        }
    }
}

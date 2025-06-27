using Autorizacion.Abstracciones.BW;
using Autorizacion.Abstracciones.Modelos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Autorizacion.Middleware
{
    public class ClaimsPerfiles
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private IAutorizacionBW _autorizacionBW;

        public ClaimsPerfiles(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext htppContext, IAutorizacionBW autorizacionBW)
        {
            _autorizacionBW = autorizacionBW;
            ClaimsIdentity appIdentity = await verifiarAutorizacion(htppContext);
            htppContext.User.AddIdentity(appIdentity);
            await _next(htppContext);
        }

        private async Task<ClaimsIdentity> verifiarAutorizacion(HttpContext htppContext)
        {
            var claims=new List<Claim>();
            if(htppContext.User!=null&& htppContext.User.Identity.IsAuthenticated)
            {
                await obtenerUsuario(htppContext, claims);
                await obtenerPerfiles(htppContext, claims);

            }
            var appIdentity = new ClaimsIdentity(claims);
            return appIdentity; 

        }

        private async Task obtenerPerfiles(HttpContext htppContext, List<Claim> claims)
        {
            var perfiles = await obtenerInformacionPerfiles(htppContext);
            if (perfiles != null && perfiles.Any())
            {
                foreach (var perfil in perfiles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, perfil.Id.ToString()));
                    

                }
            }
        }

        private async Task<IEnumerable<Perfiles>> obtenerInformacionPerfiles(HttpContext htppContext)
        {
            return await _autorizacionBW.ObtenerPerfilesUsuario(new Abstracciones.Modelos.Usuario
            {
                NombreUsuario = htppContext.User.Claims.Where(c => c.Type == "usuario").FirstOrDefault().Value
            });
        }

        private async Task obtenerUsuario(HttpContext htppContext, List<Claim> claims)
        {
            var usuario=await obtenerInformacionUsuario(htppContext);
            if(usuario is not null&& !string.IsNullOrEmpty(usuario.Id.ToString())&& !string.IsNullOrEmpty(usuario.NombreUsuario.ToString())&& !string.IsNullOrEmpty(usuario.CorreoElectronico.ToString()))
            {
                claims.Add(new Claim(ClaimTypes.Email, usuario.CorreoElectronico));
                claims.Add(new Claim(ClaimTypes.Name, usuario.NombreUsuario));
                claims.Add(new Claim("IdUsuario", usuario.Id.ToString()));
            }
        }

        private async Task<Usuario> obtenerInformacionUsuario(HttpContext htppContext)
        {
            return await _autorizacionBW.ObtenerUsuario(new Abstracciones.Modelos.Usuario
            {
                NombreUsuario = htppContext.User.Claims.Where(c => c.Type == "usuario").FirstOrDefault().Value
            });
        }
    }
    public static class ClaimsPerfilesExtensions
    {
        public static IApplicationBuilder UseClaimsPerfiles(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClaimsPerfiles>();
        }
    }

}

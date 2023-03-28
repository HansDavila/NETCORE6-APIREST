using APICORE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace APICORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {

        private readonly string _rutaServidor;
        private readonly string _cadenaSql;

        public DocumentoController(IConfiguration config)
        {
            _rutaServidor = config.GetSection("Configuracion").GetSection("RutaServidor").Value;
            _cadenaSql = config.GetConnectionString("CadenaSQL");
        }

        [HttpPost]
        [Route("Subir")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public IActionResult Subir([FromForm] Documento request)
        {
            string rutaDocumento = Path.Combine(_rutaServidor, request.Archivo.FileName);
            try
            {
                //Guardar documento en ruta del servidor
                using (FileStream newFile = System.IO.File.Create(rutaDocumento))
                {
                    request.Archivo.CopyTo(newFile);
                    newFile.Flush();
                }

                using(var conexion = new SqlConnection(_cadenaSql))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_documento", conexion);
                    cmd.Parameters.AddWithValue("descripcion", request.Descripcion);
                    cmd.Parameters.AddWithValue("ruta", rutaDocumento);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "documento guardado" });
            }catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}

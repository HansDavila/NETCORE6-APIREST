namespace APICORE.Models
{
    public class Documento
    {
        public int IdDocuemnto { get; set; }

        public string Descripcion { get; set; }

        public string Ruta { get; set; }

        public IFormFile Archivo { get; set; }
    }
}

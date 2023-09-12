using System.Net;

namespace MagicVilla_API.Modelos
{
    public class ApiResponse
    {
        public HttpStatusCode statusCode { get; set; }
        public bool IsExistoso { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = null!;
        public object Resultado { get; set; } = null!;

    }
}

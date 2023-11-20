using Domain.Model;

namespace Domain.Json
{
    public class ListaCarrosRequest
    {
        public DateTime DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

    }
}
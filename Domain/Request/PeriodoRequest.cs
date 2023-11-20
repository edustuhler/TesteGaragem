using Domain.Model;

namespace Domain.Request
{
    public class PeriodoRequest
    {
        public DateTime DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

    }
}
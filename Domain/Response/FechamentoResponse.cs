using Domain.Model;

namespace Domain.Response
{
    public class FechamentoResponse
    {
        public ICollection<Fechamento> FormasPagamentos { get; set; } = new HashSet<Fechamento>();
        public decimal Total { get; set; }

    }
}
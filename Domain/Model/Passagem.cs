namespace Domain.Model
{
    public class Passagem
    {
        public string Garagem { get; set; }
        public string CarroPlaca { get; set; }
        public string CarroMarca { get; set; }
        public string CarroModelo { get; set; }
        public string DataHoraEntrada { get; set; }
        public string? DataHoraSaida { get; set; }
        public string FormaPagamento { get; set; }
        public string PrecoTotal { get; set; }

    }
}
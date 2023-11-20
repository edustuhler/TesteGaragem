using Domain.Json;
using Domain.Model;
using Domain.Request;
using Domain.Response;
using Newtonsoft.Json;
using System.Text;

namespace Application.Commands
{
    public static class PassagemCommand
    {

        public static ICollection<Passagem> CalcularPassagem()
        {
            var jsonGaragens = File.ReadAllText(Path.Combine("Assets", "Garagens.json"), Encoding.UTF8);
            var jsonPassagens = File.ReadAllText(Path.Combine("Assets", "Passagens.json"), Encoding.UTF8);

            GaragemJson garagens = JsonConvert.DeserializeObject<GaragemJson>(jsonGaragens);

            PassagemJson passagens = JsonConvert.DeserializeObject<PassagemJson>(jsonPassagens);

            foreach (var passagem in passagens.Passagens)
            {
                var dataHoraSaida = !string.IsNullOrWhiteSpace(passagem.DataHoraSaida) ? DateTime.Parse(passagem.DataHoraSaida) : DateTime.Now;
                var dataHoraEntrada = DateTime.Parse(passagem.DataHoraEntrada);
                var diffHora = (dataHoraSaida - dataHoraEntrada).TotalMinutes;
                var precoTotal = 0m;
                if (passagem.FormaPagamento != "MEN" && diffHora >= 60)
                {
                    var garagem = garagens?.Garagens.FirstOrDefault(e => e.Codigo == passagem.Garagem) ?? throw new Exception("Garagem não encontrada");
                    precoTotal = decimal.Parse(garagem.Preco_1aHora);
                    diffHora -= 60;
                    while (diffHora >= 30)
                    {
                        precoTotal += decimal.Parse(garagem.Preco_HorasExtra);
                        diffHora -= 60;
                    }
                }
                passagem.PrecoTotal = precoTotal.ToString("N2");
            }
            return passagens.Passagens;
        }
        public static FechamentoResponse CalcularFechamento(PeriodoRequest periodoRequest)
        {
            var jsonGaragens = File.ReadAllText(Path.Combine("Assets", "Garagens.json"), Encoding.UTF8);
            var jsonPassagens = File.ReadAllText(Path.Combine("Assets", "Passagens.json"), Encoding.UTF8);
            var jsonFormasPagamento = File.ReadAllText(Path.Combine("Assets", "FormasPagamento.json"), Encoding.UTF8);

            GaragemJson garagens = JsonConvert.DeserializeObject<GaragemJson>(jsonGaragens);
            PassagemJson passagens = JsonConvert.DeserializeObject<PassagemJson>(jsonPassagens);
            FormasPagamentoJson formasPagamento = JsonConvert.DeserializeObject<FormasPagamentoJson>(jsonFormasPagamento);

            var passagensFiltradas = passagens.Passagens
                        .Where(e => DateTime.Parse(e.DataHoraEntrada) >= periodoRequest.DataInicial)
                        .Where(e => DateTime.Parse(e.DataHoraSaida) <= periodoRequest.DataFinal)
                        ;
            var fechamento = new FechamentoResponse();
            var fechamentos = new List<Fechamento>();
            fechamento.Total = 0;
            foreach (var grupo in passagensFiltradas.GroupBy(e => e.FormaPagamento))
            {
                var formaPagamento = new Fechamento();
                formaPagamento.FormaPagamento = grupo.Key;
                formaPagamento.FormaPagamentoDescricao = formasPagamento?.FormasPagamento.FirstOrDefault(e => e.Codigo == grupo.Key)?.Descricao ?? throw new Exception("Forma de Pagamento não encontrada");
                formaPagamento.Valor = 0;
                if (grupo.Key == "MEN")
                {
                    foreach (var carro in grupo.DistinctBy(e => e.CarroPlaca))
                    {
                        formaPagamento.Valor += decimal.Parse(garagens?.Garagens.FirstOrDefault(e => e.Codigo == carro.Garagem)?.Preco_Mensalista ?? "0");
                    }
                }
                else
                {
                    foreach (var passagem in grupo)
                    {
                        var dataHoraSaida = !string.IsNullOrWhiteSpace(passagem.DataHoraSaida) ? DateTime.Parse(passagem.DataHoraSaida) : DateTime.Now;
                        var dataHoraEntrada = DateTime.Parse(passagem.DataHoraEntrada);
                        var diffHora = (dataHoraSaida - dataHoraEntrada).TotalMinutes;
                        var precoTotal = 0m;
                        if (passagem.FormaPagamento != "MEN" && diffHora >= 60)
                        {
                            var garagem = garagens?.Garagens.FirstOrDefault(e => e.Codigo == passagem.Garagem) ?? throw new Exception("Garagem não encontrada");
                            precoTotal = decimal.Parse(garagem.Preco_1aHora);
                            diffHora -= 60;
                            while (diffHora >= 30)
                            {
                                precoTotal += decimal.Parse(garagem.Preco_HorasExtra);
                                diffHora -= 60;
                            }
                        }
                        formaPagamento.Valor = precoTotal;
                    }
                }
                fechamentos.Add(formaPagamento);

            }

            fechamento.FormasPagamentos = fechamentos;
            fechamento.Total = fechamentos.Sum(e => e.Valor);

            return fechamento;
        }
    public static TempoMedioResponse CalcularTempoMedio(PeriodoRequest periodoRequest)
        {
            var jsonGaragens = File.ReadAllText(Path.Combine("Assets", "Garagens.json"), Encoding.UTF8);
            var jsonPassagens = File.ReadAllText(Path.Combine("Assets", "Passagens.json"), Encoding.UTF8);

            GaragemJson garagens = JsonConvert.DeserializeObject<GaragemJson>(jsonGaragens);
            PassagemJson passagens = JsonConvert.DeserializeObject<PassagemJson>(jsonPassagens);

            var tempoMedio = new TempoMedioResponse();

            var passagensMensalistas = passagens.Passagens
                        .Where(e => DateTime.Parse(e.DataHoraEntrada) >= periodoRequest.DataInicial)
                        .Where(e => DateTime.Parse(e.DataHoraSaida) <= periodoRequest.DataFinal)
                        .Where(e => e.FormaPagamento == "MEN").GroupBy(e => e.CarroPlaca);
            tempoMedio.Mensalista = (decimal)(passagensMensalistas.ToList().Sum(e => e.Sum(e => (DateTime.Parse(e.DataHoraSaida) - DateTime.Parse(e.DataHoraEntrada)).TotalMinutes))/passagensMensalistas.Count());
            
            var passagensNaoMensalistas = passagens.Passagens
                        .Where(e => DateTime.Parse(e.DataHoraEntrada) >= periodoRequest.DataInicial)
                        .Where(e => DateTime.Parse(e.DataHoraSaida) <= periodoRequest.DataFinal)
                        .Where(e => e.FormaPagamento != "MEN").GroupBy(e => e.CarroPlaca);
            tempoMedio.NaoMensalista = (decimal)(passagensNaoMensalistas.ToList().Sum(e => e.Sum(e => (DateTime.Parse(e.DataHoraSaida) - DateTime.Parse(e.DataHoraEntrada)).TotalMinutes))/ passagensNaoMensalistas.Count());



            return tempoMedio;
        }
    }
}
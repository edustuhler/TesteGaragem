using Domain.Json;
using Domain.Model;
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
    }
}
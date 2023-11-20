using Domain.Json;
using Domain.Enum;
using Domain.Model;
using Newtonsoft.Json;
using System.Text;

namespace Application.Commands
{
    public static class CarroCommand
    {

        public static ICollection<Carro> ListarCarros(DateTime? dataInicial, DateTime? dataFinal, CarroTipoFiltro carroTipoFiltro)
        {
            var jsonPassagens = File.ReadAllText(Path.Combine("Assets", "Passagens.json"), Encoding.UTF8);

            PassagemJson passagens = JsonConvert.DeserializeObject<PassagemJson>(jsonPassagens);
            var carros = new List<Carro>();
            var data = DateTime.Parse(passagens.Passagens.First().DataHoraSaida);
            var passagensFiltradas = new List<Passagem>();
            switch (carroTipoFiltro)
            {
                case CarroTipoFiltro.NaGaragem:
                    passagensFiltradas = passagens.Passagens
                        .Where(e => string.IsNullOrWhiteSpace(e.DataHoraSaida))
                        .DistinctBy(e => e.CarroPlaca)
                        .ToList();
                    break;
                case CarroTipoFiltro.Passaram:
                    passagensFiltradas = passagens.Passagens
                        .Where(e => DateTime.Parse(e.DataHoraSaida) <= dataFinal)
                        .DistinctBy(e => e.CarroPlaca)
                        .ToList();
                    break;
                case CarroTipoFiltro.Periodo:
                    passagensFiltradas = passagens.Passagens
                        .Where(e => DateTime.Parse(e.DataHoraEntrada) >= dataInicial)
                        .Where(e => DateTime.Parse(e.DataHoraSaida) <= dataFinal)
                        .DistinctBy(e => e.CarroPlaca)
                        .ToList();
                    break;
            }
            foreach (var passagem in passagensFiltradas.ToList())
            {
                carros.Add(new Carro
                {
                    CarroMarca = passagem.CarroMarca,
                    CarroModelo = passagem.CarroModelo,
                    CarroPlaca = passagem.CarroPlaca
                });
            }

            return carros;
        }
    }
}
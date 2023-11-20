using Application;
using Application.Commands;
using Domain.Enum;
using Domain.Json;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace TesteGaragem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesteGaragemController : ControllerBase
    {

        private readonly ILogger<TesteGaragemController> _logger;

        public TesteGaragemController(ILogger<TesteGaragemController> logger)
        {
            _logger = logger;
        }

        [HttpGet("passagens")]
        public ICollection<Passagem> ListaPassagens()
        {
            return PassagemCommand.CalcularPassagem();
        }
        [HttpPost("carrosperiodo")]
        public ICollection<Carro> ListarCarrosPeriodo([FromBody] ListaCarrosRequest listaCarrosRequest)
        {
            return CarroCommand.ListarCarros(listaCarrosRequest.DataInicial, listaCarrosRequest.DataFinal, CarroTipoFiltro.Periodo);
        }
        [HttpPost("carrosgaragem")]
        public ICollection<Carro> ListarCarrosGaragem([FromBody] ListaCarrosRequest listaCarrosRequest)
        {
            return CarroCommand.ListarCarros(listaCarrosRequest.DataInicial, null, CarroTipoFiltro.NaGaragem);
        }
        [HttpPost("carrospassaram")]
        public ICollection<Carro> ListarCarrosPassaram()
        {
            return CarroCommand.ListarCarros(null, DateTime.Now.AddSeconds(-1), CarroTipoFiltro.Passaram);
        }
    }
}
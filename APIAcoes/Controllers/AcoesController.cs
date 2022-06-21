using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using StackExchange.Redis;
using APIAcoes.Models;
using APIAcoes.Extensions;

namespace APIAcoes.Controllers;

[ApiController]
[Route("[controller]")]
public class AcoesController : ControllerBase
{
    private readonly ILogger<AcoesController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ConnectionMultiplexer _redisConnection;

    public AcoesController(ILogger<AcoesController> logger,
        IConfiguration configuration,
        ConnectionMultiplexer redisConnection)
    {
        _logger = logger;
        _configuration = configuration;
        _redisConnection = redisConnection;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Resultado), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<Resultado> Post(Acao acao)
    {
        CotacaoAcao cotacaoAcao = new ()
        {
            Codigo = acao.Codigo,
            Valor = acao.Valor,
            CodCorretora = _configuration["Corretora:Codigo"],
            NomeCorretora = _configuration["Corretora:Nome"]
        };
        var conteudoAcao = JsonSerializer.Serialize(cotacaoAcao);
        _logger.LogInformation($"Dados: {conteudoAcao}");

        string topic = _configuration["ApacheKafka:Topic"];

        using (var producer = KafkaExtensions.CreateProducer(_configuration))
        {
            var result = await producer.ProduceAsync(
                topic,
                new Message<Null, string>
                { Value = conteudoAcao });

            _logger.LogInformation(
                $"Apache Kafka - Envio para o tópico {topic} concluído | " +
                $"{conteudoAcao} | Status: { result.Status.ToString()}");
        }

        return new ()
        {
            Mensagem = "Informações de ação enviadas com sucesso!"
        };
    }

    [HttpGet("{codigo}")]
    [ProducesResponseType(typeof(UltimaCotacao), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Resultado), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public ActionResult<UltimaCotacao> Get(string codigo)
    {
        var dbRedis = _redisConnection.GetDatabase();
        var acaoKey = $"ACAO-{codigo?.Trim().ToUpper()}";
        if (dbRedis.HashExists(acaoKey, "Codigo"))
        {
            var resultado = new UltimaCotacao();
            resultado.Codigo = dbRedis.HashGet(acaoKey, "Codigo");
            resultado.Valor = JsonSerializer.Deserialize<double>(
                dbRedis.HashGet(acaoKey, "Valor")!);
            resultado.DataReferencia = dbRedis.HashGet(acaoKey, "DataReferencia");
            resultado.CodCorretora = dbRedis.HashGet(acaoKey, "CodCorretora");
            resultado.NomeCorretora = dbRedis.HashGet(acaoKey, "NomeCorretora");

            _logger.LogInformation(
                $"Valor mais recente para a ação {resultado.Codigo}: {resultado.Valor}");
            
            return resultado;
        }
        else
        {
            _logger.LogError($"Informações não encontradas para a ação: {codigo}");
            return NotFound();
        }
    }
}
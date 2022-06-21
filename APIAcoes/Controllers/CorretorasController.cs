using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using APIAcoes.Models;

namespace APIAcoes.Controllers;

[ApiController]
[Route("[controller]")]
public class CorretorasController : ControllerBase
{
    [HttpGet]
    public Corretora Get(
        [FromServices] IConfiguration configuration,
        [FromServices]ILogger<AcoesController> logger)
    {
        Corretora corretora = new ()
        {
            Codigo = configuration["Corretora:Codigo"],
            Nome = configuration["Corretora:Nome"]
        };

        logger.LogInformation($"Dados: {JsonSerializer.Serialize(corretora)}");

        return corretora;
    }        
}
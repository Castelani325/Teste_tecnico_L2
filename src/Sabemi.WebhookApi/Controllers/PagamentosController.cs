using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sabemi.WebhookApi.Data;
using Sabemi.WebhookApi.Dtos;

namespace Sabemi.WebhookApi.Controllers;

[ApiController]
[Route("pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly SabemiDbContext _db;
    private readonly ILogger<PagamentosController> _logger;

    public PagamentosController(SabemiDbContext db, ILogger<PagamentosController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PagamentoResponse>>> Listar(
        [FromQuery] string? status,
        [FromQuery(Name = "id_contrato")] string? idContrato)
    {
        var query = _db.EventosBrutos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status.ToLower() == status.ToLower());

        if (!string.IsNullOrWhiteSpace(idContrato))
            query = query.Where(e => e.IdContrato == idContrato);

        var resultado = await query
            .OrderByDescending(e => e.RecebidoEm)
            .Select(e => new PagamentoResponse
            {
                IdTransacao = e.IdTransacao,
                IdContrato = e.IdContrato,
                Valor = e.Valor,
                DataPagamento = e.DataPagamento,
                Status = e.Status,
                RecebidoEm = e.RecebidoEm
            })
            .ToListAsync();

        _logger.LogInformation(
            "Listagem de pagamentos: {Quantidade} resultado(s) (status={Status}, id_contrato={IdContrato})",
            resultado.Count, status, idContrato);

        return Ok(resultado);
    }
}

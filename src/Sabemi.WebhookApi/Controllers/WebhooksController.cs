using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sabemi.WebhookApi.BackgroundProcessing;
using Sabemi.WebhookApi.Data;
using Sabemi.WebhookApi.Filters;
using Sabemi.WebhookApi.Models;
using System.Text.Json;

namespace Sabemi.WebhookApi.Controllers;

[ApiController]
[Route("webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly SabemiDbContext _db;
    private readonly PagamentoProcessingQueue _queue;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(SabemiDbContext db, PagamentoProcessingQueue queue, ILogger<WebhooksController> logger)
    {
        _db = db;
        _queue = queue;
        _logger = logger;
    }


    [HttpPost("pagamento")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public async Task<IActionResult> ReceberPagamento([FromBody] PagamentoWebhookRequest request)
    {
        // 1) idempotência "otimista": evita gravar de novo em requests repetidos
        var jaExiste = await _db.EventosBrutos
            .AnyAsync(e => e.IdTransacao == request.IdTransacao);

        if (jaExiste)
        {
            _logger.LogInformation("Evento duplicado ignorado: {IdTransacao}", request.IdTransacao);
            return Ok(new
            {
                mensagem = "Evento já recebido anteriormente, notificação ignorada (idempotência).",
                id_transacao = request.IdTransacao
            });
        }

        var evento = new EventoBruto
        {
            IdTransacao = request.IdTransacao,
            IdContrato = request.IdContrato,
            Valor = request.Valor,
            DataPagamento = request.DataPagamento,
            Status = request.Status,
            PayloadBruto = JsonSerializer.Serialize(request),
            RecebidoEm = DateTime.UtcNow
        };

        _db.EventosBrutos.Add(evento);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // 2) rede de segurança contra corrida: dois requests concorrentes com o mesmo id_transacao
            _logger.LogWarning("Corrida de idempotência detectada para {IdTransacao}", request.IdTransacao);
            return Ok(new
            {
                mensagem = "Evento já recebido anteriormente, notificação ignorada (idempotência).",
                id_transacao = request.IdTransacao
            });
        }

        _logger.LogInformation("Evento {IdTransacao} recebido e persistido.", request.IdTransacao);

        // Enfileira pro PagamentoBackgroundService processar (2s simulados) e
        // atualizar status_contrato - sem bloquear a resposta ao banco.
        await _queue.EnfileirarAsync(new PagamentoProcessamentoItem(
            evento.IdTransacao, evento.IdContrato, evento.Status));

        // 202: já persistimos rápido; o processamento pesado acontece em background.
        return Accepted(new
        {
            mensagem = "Evento recebido com sucesso. Processamento em andamento.",
            id_transacao = request.IdTransacao
        });

    }
}

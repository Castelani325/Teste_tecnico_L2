using Sabemi.WebhookApi.Data;
using Sabemi.WebhookApi.Models;

namespace Sabemi.WebhookApi.BackgroundProcessing;

/// <summary>
/// Consome a fila de pagamentos recebidos e atualiza status_contrato,
/// simulando uma regra de negocio pesada (2s) fora do ciclo de request/response
/// do webhook - e assim o endpoint POST /webhooks/pagamento responde rapido.
/// </summary>
public class PagamentoBackgroundService : BackgroundService
{
    private static readonly TimeSpan TempoProcessamentoSimulado = TimeSpan.FromSeconds(2);

    private readonly PagamentoProcessingQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PagamentoBackgroundService> _logger;

    public PagamentoBackgroundService(
        PagamentoProcessingQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<PagamentoBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PagamentoBackgroundService iniciado, aguardando eventos...");

        await foreach (var item in _queue.LerTodosAsync(stoppingToken))
        {
            try
            {
                await ProcessarAsync(item, stoppingToken);
            }
            catch (Exception ex)
            {
                // Um item com falha nao pode derrubar o worker inteiro -
                // os demais itens da fila precisam continuar sendo processados.
                _logger.LogError(ex, "Falha ao processar evento {IdTransacao} do contrato {IdContrato}",
                    item.IdTransacao, item.IdContrato);
            }
        }
    }

    private async Task ProcessarAsync(PagamentoProcessamentoItem item, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processando {IdTransacao} (contrato {IdContrato})...",
            item.IdTransacao, item.IdContrato);

        // Simula processamento pesado da regra de negocio (validacoes, integracao
        // com outros sistemas, etc.) - requisito de "Resiliencia" do enunciado.
        await Task.Delay(TempoProcessamentoSimulado, stoppingToken);

        // BackgroundService e singleton; SabemiDbContext e scoped -> precisa
        // criar um escopo novo por item pra resolver o DbContext corretamente.
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SabemiDbContext>();

        var statusContrato = await db.StatusContratos.FindAsync(new object[] { item.IdContrato }, stoppingToken);

        if (statusContrato is null)
        {
            db.StatusContratos.Add(new StatusContrato
            {
                IdContrato = item.IdContrato,
                StatusAtual = item.Status,
                UltimaAtualizacao = DateTime.UtcNow,
                UltimoIdTransacao = item.IdTransacao
            });
        }
        else
        {
            statusContrato.StatusAtual = item.Status;
            statusContrato.UltimaAtualizacao = DateTime.UtcNow;
            statusContrato.UltimoIdTransacao = item.IdTransacao;
        }

        await db.SaveChangesAsync(stoppingToken);

        _logger.LogInformation("Contrato {IdContrato} atualizado para status '{Status}' (evento {IdTransacao}).",
            item.IdContrato, item.Status, item.IdTransacao);
    }
}

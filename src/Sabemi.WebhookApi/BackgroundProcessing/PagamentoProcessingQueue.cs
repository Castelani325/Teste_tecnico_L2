using System.Threading.Channels;

namespace Sabemi.WebhookApi.BackgroundProcessing;

/// <summary>
/// Dados minimos necessarios pro worker processar - propositalmente desacoplado
/// da entidade EF (EventoBruto), que pertence ao DbContext scoped do controller.
/// </summary>
public record PagamentoProcessamentoItem(string IdTransacao, string IdContrato, string Status);

/// <summary>
/// Fila em memoria (producer/consumer) entre o WebhooksController e o
/// PagamentoBackgroundService. Singleton por natureza: existe uma unica
/// instancia compartilhada durante a vida do processo.
/// </summary>
public class PagamentoProcessingQueue
{
    private readonly Channel<PagamentoProcessamentoItem> _channel =
        Channel.CreateUnbounded<PagamentoProcessamentoItem>();

    public async ValueTask EnfileirarAsync(PagamentoProcessamentoItem item, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(item, ct);
    }

    public IAsyncEnumerable<PagamentoProcessamentoItem> LerTodosAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}

namespace Sabemi.WebhookApi.Models;

public class StatusContrato
{
    public string IdContrato { get; set; } = string.Empty;
    public string StatusAtual { get; set; } = string.Empty;
    public DateTime UltimaAtualizacao { get; set; }
    public string? UltimoIdTransacao { get; set; }
}

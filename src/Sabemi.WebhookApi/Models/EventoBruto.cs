namespace Sabemi.WebhookApi.Models;

public class EventoBruto
{
    public int Id { get; set; }
    public string IdTransacao { get; set; } = string.Empty;
    public string IdContrato { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PayloadBruto { get; set; }
    public DateTime RecebidoEm { get; set; }
}


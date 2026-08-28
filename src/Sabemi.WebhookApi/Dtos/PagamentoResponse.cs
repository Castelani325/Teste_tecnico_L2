using System.Text.Json.Serialization;

namespace Sabemi.WebhookApi.Dtos;

public class PagamentoResponse
{
    [JsonPropertyName("id_transacao")]
    public string IdTransacao { get; set; } = string.Empty;

    [JsonPropertyName("id_contrato")]
    public string IdContrato { get; set; } = string.Empty;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("data_pagamento")]
    public DateTime DataPagamento { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("recebido_em")]
    public DateTime RecebidoEm { get; set; }
}

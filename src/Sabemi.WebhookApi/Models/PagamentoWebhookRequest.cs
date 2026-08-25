using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sabemi.WebhookApi.Models;

public class PagamentoWebhookRequest
{
    [Required(ErrorMessage = "id_transacao é obrigatório")]
    [JsonPropertyName("id_transacao")]
    public string IdTransacao { get; set; } = string.Empty;

    [Required(ErrorMessage = "id_contrato é obrigatório")]
    [JsonPropertyName("id_contrato")]
    public string IdContrato { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "valor deve ser maior que zero")]
    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "data_pagamento é obrigatório")]
    [JsonPropertyName("data_pagamento")]
    public DateTime DataPagamento { get; set; }

    [Required(ErrorMessage = "status é obrigatório")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

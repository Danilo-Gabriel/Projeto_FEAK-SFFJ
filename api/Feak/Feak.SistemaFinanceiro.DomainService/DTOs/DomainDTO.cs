using System.Text.Json.Serialization;

namespace Feak.SistemaFinanceiro.DomainService.DTOs;

public class DomainDTO
{
    [JsonPropertyName("id")]
    public string Code { get; set; }
    [JsonPropertyName("nome")]
    public string Value { get; set; }
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
}
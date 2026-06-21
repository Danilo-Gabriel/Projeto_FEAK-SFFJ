using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs.Request;

public class ConsumidorFinalRequest
{
    [Required(ErrorMessage = "Nome obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Nome obrigatório")]
    public string Nome { get; set; } = string.Empty;
}

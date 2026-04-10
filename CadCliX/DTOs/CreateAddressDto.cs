using System.ComponentModel.DataAnnotations;

namespace CadCliX.DTOs;

/// <summary>
/// DTO para criação de endereço
/// </summary>
public class CreateAddressDto
{
    /// <summary>
    /// Nome da rua
    /// </summary>
    /// <example>Rua das Flores</example>
    [Required(ErrorMessage = "Rua é obrigatória")]
    public string Street { get; set; } = null!;

    /// <summary>
    /// Número do endereço
    /// </summary>
    /// <example>123</example>
    [Required(ErrorMessage = "Número é obrigatório")]
    public string Number { get; set; } = null!;

    /// <summary>
    /// Complemento (opcional)
    /// </summary>
    /// <example>Apto 45</example>
    public string? Complement { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    /// <example>Centro</example>
    [Required(ErrorMessage = "Bairro é obrigatório")]
    public string Neighborhood { get; set; } = null!;

    /// <summary>
    /// Cidade
    /// </summary>
    /// <example>Rio de Janeiro</example>
    [Required(ErrorMessage = "Cidade é obrigatória")]
    public string City { get; set; } = null!;

    /// <summary>
    /// Estado
    /// </summary>
    /// <example>RJ</example>
    [Required(ErrorMessage = "Estado é obrigatório")]
    public string State { get; set; } = null!;

    /// <summary>
    /// País
    /// </summary>
    /// <example>Brasil</example>
    [Required(ErrorMessage = "País é obrigatório")]
    public string Country { get; set; } = null!;

    /// <summary>
    /// CEP
    /// </summary>
    /// <example>20000-000</example>
    [Required(ErrorMessage = "CEP é obrigatório")]
    public string ZipCode { get; set; } = null!;
}

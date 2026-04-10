using CadCliX.Enums;
using System.ComponentModel.DataAnnotations;

namespace CadCliX.DTOs;

/// <summary>
/// DTO para criação de cliente
/// </summary>
public class CreateCustomerDto
{
    /// <summary>
    /// Primeiro nome do cliente
    /// </summary>
    /// <example>João</example>
    [Required(ErrorMessage = "Primeiro nome é obrigatório")]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Sobrenome do cliente
    /// </summary>
    /// <example>Silva</example>
    [Required(ErrorMessage = "Sobrenome é obrigatório")]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Nome da empresa
    /// </summary>
    /// <example>Empresa XYZ Ltda</example>
    [Required(ErrorMessage = "Nome da empresa é obrigatório")]
    public string Company { get; set; } = null!;

    /// <summary>
    /// Tipo de pessoa (1 = Física, 2 = Jurídica)
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Tipo de pessoa é obrigatório")]
    public TipoPessoa TipoPessoa { get; set; }

    /// <summary>
    /// RG do cliente
    /// </summary>
    /// <example>12.345.678-9</example>
    [Required(ErrorMessage = "RG é obrigatório")]
    public string RG { get; set; } = null!;

    /// <summary>
    /// CPF do cliente
    /// </summary>
    /// <example>123.456.789-00</example>
    [Required(ErrorMessage = "CPF é obrigatório")]
    public string Cpf { get; set; } = null!;

    /// <summary>
    /// CNPJ (obrigatório apenas para pessoa jurídica)
    /// </summary>
    /// <example>12.345.678/0001-99</example>
    public string? Cnpj { get; set; }

    /// <summary>
    /// ID do endereço (deve existir previamente)
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "ID do endereço é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "ID do endereço deve ser maior que zero")]
    public int AddressId { get; set; }
}

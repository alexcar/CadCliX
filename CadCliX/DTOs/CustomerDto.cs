using CadCliX.Enums;

namespace CadCliX.DTOs;

/// <summary>
/// DTO de retorno de cliente
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// ID do cliente
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Primeiro nome
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Sobrenome
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Nome da empresa
    /// </summary>
    public string Company { get; set; } = null!;

    /// <summary>
    /// Tipo de pessoa (1 = Física, 2 = Jurídica)
    /// </summary>
    public TipoPessoa TipoPessoa { get; set; }

    /// <summary>
    /// RG
    /// </summary>
    public string RG { get; set; } = null!;

    /// <summary>
    /// CPF
    /// </summary>
    public string Cpf { get; set; } = null!;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string? Cnpj { get; set; }

    /// <summary>
    /// ID do endereço
    /// </summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Dados do endereço
    /// </summary>
    public AddressDto? Address { get; set; }
}

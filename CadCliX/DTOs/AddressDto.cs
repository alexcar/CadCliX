namespace CadCliX.DTOs;

/// <summary>
/// DTO de retorno de endereço
/// </summary>
public class AddressDto
{
    /// <summary>
    /// ID do endereço
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome da rua
    /// </summary>
    public string Street { get; set; } = null!;

    /// <summary>
    /// Número do endereço
    /// </summary>
    public string Number { get; set; } = null!;

    /// <summary>
    /// Complemento
    /// </summary>
    public string? Complement { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    public string Neighborhood { get; set; } = null!;

    /// <summary>
    /// Cidade
    /// </summary>
    public string City { get; set; } = null!;

    /// <summary>
    /// Estado
    /// </summary>
    public string State { get; set; } = null!;

    /// <summary>
    /// País
    /// </summary>
    public string Country { get; set; } = null!;

    /// <summary>
    /// CEP
    /// </summary>
    public string ZipCode { get; set; } = null!;
}

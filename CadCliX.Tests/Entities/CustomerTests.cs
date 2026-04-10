using CadCliX.Entities;
using CadCliX.Enums;
using FluentAssertions;

namespace CadCliX.Tests.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidDataPessoaFisica_ShouldReturnSuccess()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.Company.Should().Be(company);
        result.Value.TipoPessoa.Should().Be(tipoPessoa);
        result.Value.RG.Should().Be(rg);
        result.Value.Cpf.Should().Be(cpf);
        result.Value.AddressId.Should().Be(addressId);
        result.Value.Active.Should().BeTrue();
    }

    [Fact]
    public void Create_WithValidDataPessoaJuridica_ShouldReturnSuccess()
    {
        // Arrange
        var firstName = "Maria";
        var lastName = "Santos";
        var company = "ABC Comércio LTDA";
        var tipoPessoa = TipoPessoa.Juridica;
        var rg = "98.765.432-1";
        var cpf = "987.654.321-00";
        var cnpj = "12.345.678/0001-99";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Cnpj.Should().Be(cnpj);
        result.Value.TipoPessoa.Should().Be(TipoPessoa.Juridica);
    }

    [Fact]
    public void Create_WithNullFirstName_ShouldReturnFailure()
    {
        // Arrange
        string firstName = null!;
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Primeiro nome é obrigatório");
    }

    [Fact]
    public void Create_WithEmptyFirstName_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "";
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Primeiro nome é obrigatório");
    }

    [Fact]
    public void Create_WithNullLastName_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "João";
        string lastName = null!;
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Último nome é obrigatório");
    }

    [Fact]
    public void Create_WithNullCompany_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        string company = null!;
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Nome da empresa é obrigatória");
    }

    [Fact]
    public void Create_WithNullRG_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        string rg = null!;
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("RG é obrigatório");
    }

    [Fact]
    public void Create_PessoaFisicaWithoutCPF_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        string cpf = null!;
        var cnpj = "";
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CPF é obrigatório para pessoa física");
    }

    [Fact]
    public void Create_PessoaJuridicaWithoutCNPJ_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "Maria";
        var lastName = "Santos";
        var company = "ABC Comércio LTDA";
        var tipoPessoa = TipoPessoa.Juridica;
        var rg = "98.765.432-1";
        var cpf = "987.654.321-00";
        string cnpj = null!;
        var addressId = 1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CNPJ é obrigatório para pessoa jurídica");
    }

    [Fact]
    public void Create_WithZeroAddressId_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = 0;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("O endereço é obrigatório");
    }

    [Fact]
    public void Create_WithNegativeAddressId_ShouldReturnFailure()
    {
        // Arrange
        var firstName = "João";
        var lastName = "Silva";
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        var cpf = "123.456.789-00";
        var cnpj = "";
        var addressId = -1;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("O endereço é obrigatório");
    }

    [Fact]
    public void Create_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        string firstName = null!;
        string lastName = null!;
        var company = "Empresa XYZ";
        var tipoPessoa = TipoPessoa.Fisica;
        var rg = "12.345.678-9";
        string cpf = null!;
        var cnpj = "";
        var addressId = 0;

        // Act
        var result = Customer.Create(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Primeiro nome é obrigatório");
        result.Error.Should().Contain("Último nome é obrigatório");
        result.Error.Should().Contain("CPF é obrigatório para pessoa física");
        result.Error.Should().Contain("O endereço é obrigatório");
    }
}

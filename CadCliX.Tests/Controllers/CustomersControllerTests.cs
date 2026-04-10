using CadCliX.Controllers;
using CadCliX.DTOs;
using CadCliX.Entities;
using CadCliX.Enums;
using CadCliX.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CadCliX.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IAddressRepository> _addressRepositoryMock;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _addressRepositoryMock = new Mock<IAddressRepository>();
        _controller = new CustomersController(_customerRepositoryMock.Object, _addressRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithCustomers()
    {
        // Arrange
        var addressResult = Address.Create("Rua 1", "1", null, "Centro", "Rio", "RJ", "Brasil", "20000-000");
        var customerResult1 = Customer.Create("João", "Silva", "Empresa 1", TipoPessoa.Fisica, "11.111.111-1", "111.111.111-11", "", 1);
        var customerResult2 = Customer.Create("Maria", "Santos", "Empresa 2", TipoPessoa.Juridica, "22.222.222-2", "222.222.222-22", "12.345.678/0001-99", 1);
        
        var customers = new List<Customer> { customerResult1.Value, customerResult2.Value };
        _customerRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<CustomerDto>>().Subject;
        returnedCustomers.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _customerRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<CustomerDto>>().Subject;
        returnedCustomers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnOkWithCustomer()
    {
        // Arrange
        var addressResult = Address.Create("Rua Teste", "123", null, "Centro", "Rio", "RJ", "Brasil", "20000-000");
        var customerResult = Customer.Create("João", "Silva", "Empresa XYZ", TipoPessoa.Fisica, "12.345.678-9", "123.456.789-00", "", 1);
        
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customerResult.Value);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCustomer = okResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.FirstName.Should().Be("João");
        returnedCustomer.LastName.Should().Be("Silva");
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Cliente não encontrado." });
    }

    [Fact]
    public async Task Create_WithValidDataPessoaFisica_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            FirstName = "João",
            LastName = "Silva",
            Company = "Empresa XYZ",
            TipoPessoa = TipoPessoa.Fisica,
            RG = "12.345.678-9",
            Cpf = "123.456.789-00",
            Cnpj = null,
            AddressId = 1
        };

        _addressRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        
        var customerResult = Customer.Create(dto.FirstName, dto.LastName, dto.Company, dto.TipoPessoa, dto.RG, dto.Cpf, dto.Cnpj ?? "", dto.AddressId);
        _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>())).ReturnsAsync(customerResult.Value);
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(customerResult.Value);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        
        var returnedCustomer = createdResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.FirstName.Should().Be("João");
        returnedCustomer.TipoPessoa.Should().Be(TipoPessoa.Fisica);
    }

    [Fact]
    public async Task Create_WithValidDataPessoaJuridica_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            FirstName = "Maria",
            LastName = "Santos",
            Company = "ABC Comércio LTDA",
            TipoPessoa = TipoPessoa.Juridica,
            RG = "98.765.432-1",
            Cpf = "987.654.321-00",
            Cnpj = "12.345.678/0001-99",
            AddressId = 1
        };

        _addressRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        
        var customerResult = Customer.Create(dto.FirstName, dto.LastName, dto.Company, dto.TipoPessoa, dto.RG, dto.Cpf, dto.Cnpj, dto.AddressId);
        _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>())).ReturnsAsync(customerResult.Value);
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(customerResult.Value);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedCustomer = createdResult.Value.Should().BeOfType<CustomerDto>().Subject;
        returnedCustomer.TipoPessoa.Should().Be(TipoPessoa.Juridica);
        returnedCustomer.Cnpj.Should().Be("12.345.678/0001-99");
    }

    [Fact]
    public async Task Create_WithNonExistingAddress_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            FirstName = "João",
            LastName = "Silva",
            Company = "Empresa XYZ",
            TipoPessoa = TipoPessoa.Fisica,
            RG = "12.345.678-9",
            Cpf = "123.456.789-00",
            AddressId = 999
        };

        _addressRepositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().BeEquivalentTo(new { message = "O endereço informado não existe." });
    }

    [Fact]
    public async Task Create_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            FirstName = "",
            LastName = "Silva",
            Company = "Empresa XYZ",
            TipoPessoa = TipoPessoa.Fisica,
            RG = "12.345.678-9",
            Cpf = "123.456.789-00",
            AddressId = 1
        };

        _addressRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_PessoaFisicaWithoutCPF_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            FirstName = "João",
            LastName = "Silva",
            Company = "Empresa XYZ",
            TipoPessoa = TipoPessoa.Fisica,
            RG = "12.345.678-9",
            Cpf = null!,
            AddressId = 1
        };

        _addressRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_PessoaJuridicaWithoutCNPJ_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            FirstName = "Maria",
            LastName = "Santos",
            Company = "ABC Comércio LTDA",
            TipoPessoa = TipoPessoa.Juridica,
            RG = "98.765.432-1",
            Cpf = "987.654.321-00",
            Cnpj = null!,
            AddressId = 1
        };

        _addressRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}

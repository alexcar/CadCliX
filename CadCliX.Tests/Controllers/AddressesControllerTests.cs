using CadCliX.Controllers;
using CadCliX.DTOs;
using CadCliX.Entities;
using CadCliX.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CadCliX.Tests.Controllers;

public class AddressesControllerTests
{
    private readonly Mock<IAddressRepository> _repositoryMock;
    private readonly AddressesController _controller;

    public AddressesControllerTests()
    {
        _repositoryMock = new Mock<IAddressRepository>();
        _controller = new AddressesController(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAddresses()
    {
        // Arrange
        var addressResult1 = Address.Create("Rua 1", "1", null, "Centro", "Rio", "RJ", "Brasil", "20000-000");
        var addressResult2 = Address.Create("Rua 2", "2", "Apto 1", "Centro", "SP", "SP", "Brasil", "01000-000");
        
        var addresses = new List<Address> { addressResult1.Value, addressResult2.Value };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(addresses);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAddresses = okResult.Value.Should().BeAssignableTo<IEnumerable<AddressDto>>().Subject;
        returnedAddresses.Should().HaveCount(2);
        returnedAddresses.First().Street.Should().Be("Rua 1");
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Address>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAddresses = okResult.Value.Should().BeAssignableTo<IEnumerable<AddressDto>>().Subject;
        returnedAddresses.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnOkWithAddress()
    {
        // Arrange
        var addressResult = Address.Create("Rua Teste", "123", "Sala 10", "Centro", "Rio", "RJ", "Brasil", "20000-000");
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(addressResult.Value);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAddress = okResult.Value.Should().BeOfType<AddressDto>().Subject;
        returnedAddress.Street.Should().Be("Rua Teste");
        returnedAddress.Number.Should().Be("123");
        returnedAddress.Complement.Should().Be("Sala 10");
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Address?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Endereço não encontrado." });
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var dto = new CreateAddressDto
        {
            Street = "Rua Nova",
            Number = "100",
            Complement = "Casa",
            Neighborhood = "Bairro Novo",
            City = "Cidade Nova",
            State = "MG",
            Country = "Brasil",
            ZipCode = "30000-000"
        };

        var addressResult = Address.Create(dto.Street, dto.Number, dto.Complement, dto.Neighborhood, dto.City, dto.State, dto.Country, dto.ZipCode);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Address>())).ReturnsAsync(addressResult.Value);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        
        var returnedAddress = createdResult.Value.Should().BeOfType<AddressDto>().Subject;
        returnedAddress.Street.Should().Be("Rua Nova");
        returnedAddress.City.Should().Be("Cidade Nova");
    }

    [Fact]
    public async Task Create_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateAddressDto
        {
            Street = "",
            Number = "100",
            Neighborhood = "Bairro",
            City = "Cidade",
            State = "MG",
            Country = "Brasil",
            ZipCode = "30000-000"
        };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_WithMissingRequiredFields_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateAddressDto
        {
            Street = null!,
            Number = null!,
            Neighborhood = "Bairro",
            City = "Cidade",
            State = "MG",
            Country = "Brasil",
            ZipCode = "30000-000"
        };

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}

using CadCliX.Data;
using CadCliX.Entities;
using CadCliX.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CadCliX.Tests.Repositories;

public class AddressRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AddressRepository _repository;

    public AddressRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new AddressRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnAddress()
    {
        // Arrange
        var addressResult = Address.Create("Rua Teste", "123", null, "Centro", "Rio de Janeiro", "RJ", "Brasil", "20000-000");
        _context.Addresses.Add(addressResult.Value);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(addressResult.Value.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Street.Should().Be("Rua Teste");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveAddresses()
    {
        // Arrange
        var address1Result = Address.Create("Rua 1", "1", null, "Centro", "Rio", "RJ", "Brasil", "20000-000");
        var address2Result = Address.Create("Rua 2", "2", null, "Centro", "Rio", "RJ", "Brasil", "20000-001");
        
        _context.Addresses.Add(address1Result.Value);
        _context.Addresses.Add(address2Result.Value);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.Active);
    }

    [Fact]
    public async Task AddAsync_ShouldAddAddressAndReturnIt()
    {
        // Arrange
        var addressResult = Address.Create("Rua Nova", "100", "Sala 10", "Centro", "São Paulo", "SP", "Brasil", "01000-000");

        // Act
        var result = await _repository.AddAsync(addressResult.Value);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        var savedAddress = await _context.Addresses.FindAsync(result.Id);
        savedAddress.Should().NotBeNull();
        savedAddress!.Street.Should().Be("Rua Nova");
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var addressResult = Address.Create("Rua Existe", "50", null, "Bairro", "Cidade", "ES", "Brasil", "29000-000");
        _context.Addresses.Add(addressResult.Value);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(addressResult.Value.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

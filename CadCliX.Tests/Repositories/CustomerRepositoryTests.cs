using CadCliX.Data;
using CadCliX.Entities;
using CadCliX.Enums;
using CadCliX.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CadCliX.Tests.Repositories;

public class CustomerRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnCustomerWithAddress()
    {
        // Arrange
        var addressResult = Address.Create("Rua Teste", "123", null, "Centro", "Rio", "RJ", "Brasil", "20000-000");
        _context.Addresses.Add(addressResult.Value);
        await _context.SaveChangesAsync();

        var customerResult = Customer.Create("João", "Silva", "Empresa XYZ", TipoPessoa.Fisica, "12.345.678-9", "123.456.789-00", "", addressResult.Value.Id);
        _context.Customers.Add(customerResult.Value);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(customerResult.Value.Id);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("João");
        result.Address.Should().NotBeNull();
        result.Address.Street.Should().Be("Rua Teste");
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
    public async Task GetAllAsync_ShouldReturnOnlyActiveCustomersWithAddresses()
    {
        // Arrange
        var addressResult = Address.Create("Rua 1", "1", null, "Centro", "Rio", "RJ", "Brasil", "20000-000");
        _context.Addresses.Add(addressResult.Value);
        await _context.SaveChangesAsync();

        var customer1Result = Customer.Create("João", "Silva", "Empresa 1", TipoPessoa.Fisica, "11.111.111-1", "111.111.111-11", "", addressResult.Value.Id);
        var customer2Result = Customer.Create("Maria", "Santos", "Empresa 2", TipoPessoa.Juridica, "22.222.222-2", "222.222.222-22", "12.345.678/0001-99", addressResult.Value.Id);
        
        _context.Customers.Add(customer1Result.Value);
        _context.Customers.Add(customer2Result.Value);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Active);
        result.Should().OnlyContain(c => c.Address != null);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCustomerAndReturnIt()
    {
        // Arrange
        var addressResult = Address.Create("Rua Nova", "100", null, "Centro", "SP", "SP", "Brasil", "01000-000");
        _context.Addresses.Add(addressResult.Value);
        await _context.SaveChangesAsync();

        var customerResult = Customer.Create("Pedro", "Oliveira", "Nova Empresa", TipoPessoa.Fisica, "33.333.333-3", "333.333.333-33", "", addressResult.Value.Id);

        // Act
        var result = await _repository.AddAsync(customerResult.Value);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        var savedCustomer = await _context.Customers.FindAsync(result.Id);
        savedCustomer.Should().NotBeNull();
        savedCustomer!.FirstName.Should().Be("Pedro");
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var addressResult = Address.Create("Rua Existe", "50", null, "Bairro", "Cidade", "ES", "Brasil", "29000-000");
        _context.Addresses.Add(addressResult.Value);
        await _context.SaveChangesAsync();

        var customerResult = Customer.Create("Ana", "Costa", "Empresa Existe", TipoPessoa.Fisica, "44.444.444-4", "444.444.444-44", "", addressResult.Value.Id);
        _context.Customers.Add(customerResult.Value);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(customerResult.Value.Id);

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

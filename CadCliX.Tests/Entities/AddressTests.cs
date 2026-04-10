using CadCliX.Entities;
using FluentAssertions;

namespace CadCliX.Tests.Entities;

public class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var complement = "Apto 45";
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, complement, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Street.Should().Be(street);
        result.Value.Number.Should().Be(number);
        result.Value.Complement.Should().Be(complement);
        result.Value.Neighborhood.Should().Be(neighborhood);
        result.Value.City.Should().Be(city);
        result.Value.State.Should().Be(state);
        result.Value.Country.Should().Be(country);
        result.Value.ZipCode.Should().Be(zipCode);
        result.Value.Active.Should().BeTrue();
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithNullStreet_ShouldReturnFailure()
    {
        // Arrange
        string street = null!;
        var number = "123";
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Rua é obrigatória");
    }

    [Fact]
    public void Create_WithEmptyStreet_ShouldReturnFailure()
    {
        // Arrange
        var street = "";
        var number = "123";
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Rua é obrigatória");
    }

    [Fact]
    public void Create_WithNullNumber_ShouldReturnFailure()
    {
        // Arrange
        var street = "Rua das Flores";
        string number = null!;
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Número é obrigatório");
    }

    [Fact]
    public void Create_WithNullNeighborhood_ShouldReturnFailure()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        string neighborhood = null!;
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Bairro é obrigatório");
    }

    [Fact]
    public void Create_WithNullCity_ShouldReturnFailure()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var neighborhood = "Centro";
        string city = null!;
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cidade é obrigatória");
    }

    [Fact]
    public void Create_WithNullState_ShouldReturnFailure()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        string state = null!;
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Estado é obrigatório");
    }

    [Fact]
    public void Create_WithNullCountry_ShouldReturnFailure()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        string country = null!;
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("País é obrigatório");
    }

    [Fact]
    public void Create_WithNullZipCode_ShouldReturnFailure()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        string zipCode = null!;

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CEP é obrigatório");
    }

    [Fact]
    public void Create_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        string street = null!;
        string number = null!;
        string neighborhood = null!;
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, null, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Rua é obrigatória");
        result.Error.Should().Contain("Número é obrigatório");
        result.Error.Should().Contain("Bairro é obrigatório");
    }

    [Fact]
    public void Create_WithNullComplement_ShouldReturnSuccess()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        string? complement = null;
        var neighborhood = "Centro";
        var city = "Rio de Janeiro";
        var state = "RJ";
        var country = "Brasil";
        var zipCode = "20000-000";

        // Act
        var result = Address.Create(street, number, complement, neighborhood, city, state, country, zipCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Complement.Should().BeNull();
    }
}

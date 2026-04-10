using CadCliX.Common;

namespace CadCliX.Entities;

public sealed class Address : EntityInt
{
    public string Street { get; private set; } = null!;
    public string Number { get; private set; } = null!;
    public string? Complement { get; private set; }
    public string Neighborhood { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string ZipCode { get; private set; } = null!;

    private Address() : base()
    {

    }

    private Address(
        string street, 
        string number, 
        string? complement, 
        string neighborhood, 
        string city, 
        string state, 
        string country,
        string zipCode)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public static Result<Address> Create(
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        string country,
        string zipCode)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(street))
        {
            errors.Add("Rua é obrigatória.");
        }
        
        if (string.IsNullOrWhiteSpace(number))
        {
            errors.Add("Número é obrigatório.");
        }
        
        if (string.IsNullOrWhiteSpace(neighborhood))
        {
            errors.Add("Bairro é obrigatório.");
        }
        
        if (string.IsNullOrWhiteSpace(city))
        {
            errors.Add("Cidade é obrigatória.");
        }
        
        if (string.IsNullOrWhiteSpace(state))
        {
            errors.Add("Estado é obrigatório.");
        }
        
        if (string.IsNullOrWhiteSpace(country))
        {
            errors.Add("País é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(zipCode))
        {
            errors.Add("CEP é obrigatório.");
        }

        if (errors.Any())
            return Result.Failure<Address>(string.Join("; ", errors));
        
        var address = new Address(street, number, complement, neighborhood, city, state, country, zipCode);
        
        return Result.Success(address);
    }
}

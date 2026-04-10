using CadCliX.Common;
using CadCliX.Enums;

namespace CadCliX.Entities;

public sealed class Customer : EntityInt
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Company { get; private set; } = null!;
    public TipoPessoa TipoPessoa { get; private set; }
    public string RG { get; private set; } = null!;
    public string Cpf { get; private set; } = null!;
    public string? Cnpj { get; private set; }
    public int AddressId { get; private set; }
    public Address Address { get; private set; } = null!;

    private Customer() : base()
    {

    }

    private Customer(
        string firstName,
        string lastName,
        string company,
        TipoPessoa tipoPessoa,
        string rg,
        string cpf,
        string cnpj,
        int addressId)
    {
        FirstName = firstName;
        LastName = lastName;
        Company = company;
        TipoPessoa = tipoPessoa;
        RG = rg;
        Cpf = cpf;
        Cnpj = cnpj;
        AddressId = addressId;
    }

    public static Result<Customer> Create(
        string firstName,
        string lastName,
        string company,
        TipoPessoa tipoPessoa,
        string rg,
        string cpf,
        string cnpj,
        int addressId)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(firstName))
        {
            errors.Add("Primeiro nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            errors.Add("Último nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(company))
        {
            errors.Add("Nome da empresa é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(rg))
        {
            errors.Add("RG é obrigatório.");
        }

        if (tipoPessoa == TipoPessoa.Fisica)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                errors.Add("CPF é obrigatório para pessoa física.");
            }
        }
        else if (tipoPessoa == TipoPessoa.Juridica)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                errors.Add("CNPJ é obrigatório para pessoa jurídica.");
            }
        }

        if (addressId <= 0)
        {
            {
                errors.Add("O endereço é obrigatório.");
            }            
        }

        if (errors.Any())
        {
            return Result.Failure<Customer>(string.Join(", ", errors));
        }

        var customer = new Customer(firstName, lastName, company, tipoPessoa, rg, cpf, cnpj, addressId);

        return Result<Customer>.Success(customer);
    }
}

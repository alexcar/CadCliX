using CadCliX.DTOs;
using CadCliX.Entities;
using CadCliX.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CadCliX.Controllers;

/// <summary>
/// Controller para gerenciamento de clientes
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAddressRepository _addressRepository;

    public CustomersController(
        ICustomerRepository customerRepository,
        IAddressRepository addressRepository)
    {
        _customerRepository = customerRepository;
        _addressRepository = addressRepository;
    }

    /// <summary>
    /// Lista todos os clientes ativos com seus endereços
    /// </summary>
    /// <returns>Lista de clientes</returns>
    /// <response code="200">Retorna a lista de clientes</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        var customerDtos = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Company = c.Company,
            TipoPessoa = c.TipoPessoa,
            RG = c.RG,
            Cpf = c.Cpf,
            Cnpj = c.Cnpj,
            AddressId = c.AddressId,
            Address = c.Address is not null ? new AddressDto
            {
                Id = c.Address.Id,
                Street = c.Address.Street,
                Number = c.Address.Number,
                Complement = c.Address.Complement,
                Neighborhood = c.Address.Neighborhood,
                City = c.Address.City,
                State = c.Address.State,
                Country = c.Address.Country,
                ZipCode = c.Address.ZipCode
            } : null
        });

        return Ok(customerDtos);
    }

    /// <summary>
    /// Busca um cliente específico por ID com seu endereço
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <returns>Dados do cliente</returns>
    /// <response code="200">Retorna o cliente encontrado</response>
    /// <response code="404">Cliente não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer is null)
        {
            return NotFound(new { message = "Cliente não encontrado." });
        }

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Company = customer.Company,
            TipoPessoa = customer.TipoPessoa,
            RG = customer.RG,
            Cpf = customer.Cpf,
            Cnpj = customer.Cnpj,
            AddressId = customer.AddressId,
            Address = customer.Address is not null ? new AddressDto
            {
                Id = customer.Address.Id,
                Street = customer.Address.Street,
                Number = customer.Address.Number,
                Complement = customer.Address.Complement,
                Neighborhood = customer.Address.Neighborhood,
                City = customer.Address.City,
                State = customer.Address.State,
                Country = customer.Address.Country,
                ZipCode = customer.Address.ZipCode
            } : null
        };

        return Ok(customerDto);
    }

    /// <summary>
    /// Cria um novo cliente (importação do sistema legado)
    /// </summary>
    /// <param name="dto">Dados do cliente</param>
    /// <returns>Cliente criado</returns>
    /// <response code="201">Cliente criado com sucesso</response>
    /// <response code="400">Dados inválidos ou endereço não existe</response>
    /// <remarks>
    /// Exemplo de requisição para Pessoa Física (TipoPessoa = 1):
    /// 
    ///     POST /api/customers
    ///     {
    ///        "firstName": "João",
    ///        "lastName": "Silva",
    ///        "company": "Empresa XYZ Ltda",
    ///        "tipoPessoa": 1,
    ///        "rg": "12.345.678-9",
    ///        "cpf": "123.456.789-00",
    ///        "addressId": 1
    ///     }
    ///     
    /// Exemplo de requisição para Pessoa Jurídica (TipoPessoa = 2):
    /// 
    ///     POST /api/customers
    ///     {
    ///        "firstName": "Maria",
    ///        "lastName": "Santos",
    ///        "company": "ABC Comércio LTDA",
    ///        "tipoPessoa": 2,
    ///        "rg": "98.765.432-1",
    ///        "cpf": "987.654.321-00",
    ///        "cnpj": "12.345.678/0001-99",
    ///        "addressId": 1
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        var addressExists = await _addressRepository.ExistsAsync(dto.AddressId);
        if (!addressExists)
        {
            return BadRequest(new { message = "O endereço informado não existe." });
        }

        var result = Customer.Create(
            dto.FirstName,
            dto.LastName,
            dto.Company,
            dto.TipoPessoa,
            dto.RG,
            dto.Cpf,
            dto.Cnpj ?? string.Empty,
            dto.AddressId
        );

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        var customer = await _customerRepository.AddAsync(result.Value);
        var createdCustomer = await _customerRepository.GetByIdAsync(customer.Id);

        var customerDto = new CustomerDto
        {
            Id = createdCustomer!.Id,
            FirstName = createdCustomer.FirstName,
            LastName = createdCustomer.LastName,
            Company = createdCustomer.Company,
            TipoPessoa = createdCustomer.TipoPessoa,
            RG = createdCustomer.RG,
            Cpf = createdCustomer.Cpf,
            Cnpj = createdCustomer.Cnpj,
            AddressId = createdCustomer.AddressId,
            Address = createdCustomer.Address is not null ? new AddressDto
            {
                Id = createdCustomer.Address.Id,
                Street = createdCustomer.Address.Street,
                Number = createdCustomer.Address.Number,
                Complement = createdCustomer.Address.Complement,
                Neighborhood = createdCustomer.Address.Neighborhood,
                City = createdCustomer.Address.City,
                State = createdCustomer.Address.State,
                Country = createdCustomer.Address.Country,
                ZipCode = createdCustomer.Address.ZipCode
            } : null
        };

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customerDto);
    }
}

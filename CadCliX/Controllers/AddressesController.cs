using CadCliX.DTOs;
using CadCliX.Entities;
using CadCliX.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CadCliX.Controllers;

/// <summary>
/// Controller para gerenciamento de endereços
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AddressesController : ControllerBase
{
    private readonly IAddressRepository _repository;

    public AddressesController(IAddressRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista todos os endereços ativos
    /// </summary>
    /// <returns>Lista de endereços</returns>
    /// <response code="200">Retorna a lista de endereços</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAll()
    {
        var addresses = await _repository.GetAllAsync();
        var addressDtos = addresses.Select(a => new AddressDto
        {
            Id = a.Id,
            Street = a.Street,
            Number = a.Number,
            Complement = a.Complement,
            Neighborhood = a.Neighborhood,
            City = a.City,
            State = a.State,
            Country = a.Country,
            ZipCode = a.ZipCode
        });

        return Ok(addressDtos);
    }

    /// <summary>
    /// Busca um endereço específico por ID
    /// </summary>
    /// <param name="id">ID do endereço</param>
    /// <returns>Dados do endereço</returns>
    /// <response code="200">Retorna o endereço encontrado</response>
    /// <response code="404">Endereço não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> GetById(int id)
    {
        var address = await _repository.GetByIdAsync(id);
        if (address is null)
        {
            return NotFound(new { message = "Endereço não encontrado." });
        }

        var addressDto = new AddressDto
        {
            Id = address.Id,
            Street = address.Street,
            Number = address.Number,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            Country = address.Country,
            ZipCode = address.ZipCode
        };

        return Ok(addressDto);
    }

    /// <summary>
    /// Cria um novo endereço (importação do sistema legado)
    /// </summary>
    /// <param name="dto">Dados do endereço</param>
    /// <returns>Endereço criado</returns>
    /// <response code="201">Endereço criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AddressDto>> Create([FromBody] CreateAddressDto dto)
    {
        var result = Address.Create(
            dto.Street,
            dto.Number,
            dto.Complement,
            dto.Neighborhood,
            dto.City,
            dto.State,
            dto.Country,
            dto.ZipCode
        );

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        var address = await _repository.AddAsync(result.Value);

        var addressDto = new AddressDto
        {
            Id = address.Id,
            Street = address.Street,
            Number = address.Number,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            Country = address.Country,
            ZipCode = address.ZipCode
        };

        return CreatedAtAction(nameof(GetById), new { id = address.Id }, addressDto);
    }
}

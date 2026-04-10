using CadCliX.Entities;

namespace CadCliX.Repositories;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(int id);
    Task<IEnumerable<Address>> GetAllAsync();
    Task<Address> AddAsync(Address address);
    Task<bool> ExistsAsync(int id);
}

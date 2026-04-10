using CadCliX.Data;
using CadCliX.Entities;
using Microsoft.EntityFrameworkCore;

namespace CadCliX.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        return await _context.Addresses.FindAsync(id);
    }

    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        return await _context.Addresses
            .Where(a => a.Active)
            .ToListAsync();
    }

    public async Task<Address> AddAsync(Address address)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Addresses.AnyAsync(a => a.Id == id);
    }
}

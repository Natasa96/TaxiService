using System.Configuration;
using DataLib.Model;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Update.Internal;

public class RideRepository : AbstractRepository<Ride>, IRideRepository
{
    public RideRepository(TaxiDbContext context) : base(context)
    {

    }

    public async Task<Ride> FindByIdAsync(int id)
    {
        return await _dbContext.Set<Ride>().Include(r => r.Driver)
            .Include(r => r.User)
            .Include(r => r.StartAddress)
            .Include(r => r.EndAddress)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public override async Task<Ride> AddAsync(Ride entity)
    {
        await base.AddAsync(entity);
        await _dbContext.Entry(entity).Reference(o => o.Driver).LoadAsync();
        await _dbContext.Entry(entity).Reference(o => o.User).LoadAsync();

        return entity;
    }

    public async Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverId)
    {
        return await _dbContext.Set<Ride>().Where(x => x.DriverId == driverId)
        .Include(r => r.StartAddress)
        .Include(r => r.EndAddress)
        .Include(r => r.Driver)
        .Include(r => r.User).ToListAsync();
    }

    public async Task<IEnumerable<Ride>> GetDriverAvailableRidesAsync()
    {
        return await _dbContext.Set<Ride>().Where(x => x.DriverId == null)
        .Include(r => r.StartAddress)
        .Include(r => r.EndAddress)
        .Include(r => r.User).ToListAsync();
    }

    public async Task<IEnumerable<Ride>> GetUserRidesAsync(int userId)
    {
        return await _dbContext.Set<Ride>().Where(x => x.UserId == userId)
        .Include(r => r.StartAddress)
        .Include(r => r.EndAddress)
        .Include(r => r.User)
        .Include(r => r.Driver).ToListAsync();
    }

    public override async Task<IEnumerable<Ride>> GetAllAsync()
    {
        return await _dbContext.Set<Ride>()
          .Include(r => r.StartAddress)
          .Include(r => r.EndAddress)
          .Include(r => r.Driver)
          .Include(r => r.User).ToListAsync();

    }
}
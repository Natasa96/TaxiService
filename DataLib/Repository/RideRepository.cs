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
    return await _dbContext.Set<Ride>().FirstOrDefaultAsync(r => r.Id == id);
  }

  public async Task<List<Ride>> GetDriverRidesAsync(int driverId)
  {
    return await _dbContext.Set<Ride>().Where(x => x.DriverId == driverId).ToListAsync();
  }

  public async Task<List<Ride>> GetUserRidesAsync(int userId)
  {
    return await _dbContext.Set<Ride>().Where(x => x.UserId == userId).ToListAsync();
  }
}
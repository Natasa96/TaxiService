using DataLib.Model;

public interface IRideRepository : IRepository<Ride>
{
  Task<Ride> FindByIdAsync(int id);
  Task<List<Ride>> GetDriverRidesAsync(int driverId);
  Task<List<Ride>> GetUserRidesAsync(int userId);
}
using DataLib.Model;

public interface IRideRepository : IRepository<Ride>
{
  Task<Ride> FindByIdAsync(int id);
  Task<Ride> GetDriverRidesAsync(int userId);
  Task<Ride> GetUserRidesAsync(int userId);
}
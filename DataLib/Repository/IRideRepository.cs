using DataLib.Model;

public interface IRideRepository : IRepository<Ride>
{
    Task<Ride> FindByIdAsync(int id);
    Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverId);
    Task<IEnumerable<Ride>> GetDriverAvailableRidesAsync();
    Task<IEnumerable<Ride>> GetUserRidesAsync(int userId);
}
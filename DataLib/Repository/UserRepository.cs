using DataLib.Model;
using Microsoft.EntityFrameworkCore;

public class UserRepository : AbstractRepository<User>, IUserRepository
{
    public TaxiDbContext _userRepository;
    public UserRepository(TaxiDbContext context) : base(context)
    {
        _userRepository = context;
    }

    public async Task<User> FindByUsernameAsync(string username)
    {
        return await _dbContext.Set<User>()
        .Include(u => u.Address).FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<ICollection<User>> GetDriverWithAverageRatingAsync()
    {

        return await _userRepository.Users.Where(u => u.Role == Roles.Driver).Include(r => r.DriverRatings).ToListAsync();
    }

}
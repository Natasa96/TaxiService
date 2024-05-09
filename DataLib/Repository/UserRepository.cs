using DataLib.Model;
using Microsoft.EntityFrameworkCore;

public class UserRepository : AbstractRepository<User>, IUserRepository
{
    public UserRepository(TaxiDbContext context) : base(context)
    {

    }

    public async Task<User> FindByUsernameAsync(string username)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.User_name == username);
    }

}
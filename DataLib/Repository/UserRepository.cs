using DataLib.Model;
using Microsoft.EntityFrameworkCore;

public class UserRepository : AbstractRepository<User>
{
    public UserRepository(TaxiDbContext context) : base(context)
    {

    }

}
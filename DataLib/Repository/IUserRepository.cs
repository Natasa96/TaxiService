using DataLib.Model;

public interface IUserRepository : IRepository<User>
{
    Task<User> FindByUsernameAsync(string username);
    Task<ICollection<User>> GetDriverWithAverageRatingAsync();

}
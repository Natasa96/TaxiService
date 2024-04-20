using DataLib.Model;

public class UserRepository
{
    public readonly IRepository<User> service;

    public UserRepository(IRepository<User> userRepository)
    {
        service = userRepository;
    }

}
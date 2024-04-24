using DataLib;
using DataLib.Model;

public class AuthService
{
  private IRepository<User> _repository;
  public AuthService(IRepository<User> repository)
  {
    _repository = repository;
  }

  public bool Register(RegisterRequest registerRequest)
  {
    Console.WriteLine(registerRequest);
    return true;
  }

  public string Login(LoginRequest loginRequest)
  {
    return loginRequest.Username;
  }

  public bool VerifyDriver(int idDriver)
  {
    //TODO <------------
    return true;
  }
}
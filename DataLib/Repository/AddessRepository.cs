using DataLib.Model;

public class AddressRepository
{
    public readonly IRepository<Address> service;

    public AddressRepository(IRepository<Address> addressRepository)
    {
        service = addressRepository;
    }

}
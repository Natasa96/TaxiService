public class DriverService
{
    private readonly IUserRepository _userRepository;
    public DriverService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<DriverResponse>> GetAllDrivers()
    {
        var drivers = await _userRepository.GetDriverWithAverageRatingAsync();

        var returnDrivers = new List<DriverResponse>() { };

        foreach (var d in drivers)
        {

            var newDriverResponce = new DriverResponse()
            {
                Id = d.Id,
                FullName = d.ToString(),
                VerificationState = d.VerificationState.ToString(),
                Rating = d.DriverRatings.Any() ?
                          (d.DriverRatings.Average(r => r.Rate)).ToString("F2") :
                          "No Ratings"
            };
            returnDrivers.Add(newDriverResponce);
        }

        return returnDrivers.ToList();
    }

}
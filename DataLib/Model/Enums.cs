namespace DataLib.Model;

public enum Roles
{
    Admin,
    Driver,
    User
}

public enum VerificationState
{
    Processing,
    Approved,
    Declined
}

public enum RideStatus
{
    Available,
    InProgress,
    Finished,
    Cancelled
}

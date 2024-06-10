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
    Declined,
    Blocked
}

public enum RideStatus
{
    Accepted = 0,
    InProgress = 1,
    Finished = 2,
    Cancelled = 3
}

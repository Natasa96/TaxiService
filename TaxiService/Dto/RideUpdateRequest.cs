using DataLib.Model;

public class RideUpdateRequest
{
    public DateTime? AcceptionTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? FinishedTime { get; set; }
    public int RideId { get; set; }
    public RideStatus status { get; set; }

}
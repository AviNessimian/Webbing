namespace Webbing.Assignment.Service.Entities;

public class NetworkEvent
{
    // For EF
    public Guid Id { get; set; }

    // Will be use to identify the sim
    public Guid SimId { get; set; }

    public Guid SessionId { get; set; }

    public long Quota { get; set; }

    // The date of the current network event
    public DateTime CreatedOnUtc { get; set; }

    public int GetPartition(int partitions)
    {
        var partition = Math.Abs(SimId.GetHashCode() % partitions);
        return partition;
    }
}
namespace Webbing.Assignment.Service.Entities;

// Will be use to store the usages of the sim
public class Usage
{
    public Guid SimId { get; set; }
    public Guid CustomerId { get; set; }
    public DateOnly Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public long TotalQuota { get; set; }
}

namespace Webbing.Assignment.Api.DTO
{
    public class UsageByCustomerDTO 
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int SimCount { get; set; }
        public long TotalUsage { get; set; }
        public int LastUpdateInDays { get; set; }
    }

    public class UsageBySimDTO
    {
        public Guid SimId { get; set; }       
        public long TotalUsage { get; set; }
    }
}

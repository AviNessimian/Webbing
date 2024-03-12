namespace Webbing.Assignment.Controllers;

[ApiController]
[Route("api")]
public class UsageController : ControllerBase
{
    private readonly ILogger<UsageController> _logger;
    private readonly ApplicationDbContext _context;

    public UsageController(ILogger<UsageController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("usages-group-by-sim")]
    [ProducesResponseType(typeof(List<UsageBySimDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsagesGroupBySim(
        Guid customerId,
        DateTime FromDate,
        DateTime ToDate)
    {
        try
        {
            if (customerId == Guid.Empty)
            {
                _logger.LogError("Invalid customerId");
                return BadRequest();
            }

            var usages = await _context.Usages
                .Where(_ =>
                    _.CustomerId == customerId
                    && _.Date >= DateOnly.FromDateTime(FromDate)
                    && _.Date <= DateOnly.FromDateTime(ToDate))
                .GroupBy(usage => usage.SimId)
                .Select(group => new UsageBySimDTO
                {
                    SimId = group.Key,
                    TotalUsage = group.Sum(usage => usage.TotalQuota)
                })
                .ToListAsync();

            return Ok(usages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Usage group by sim");
            return BadRequest();
        }
    }

    [HttpGet("usages-group-by-customer")]
    [ProducesResponseType(typeof(List<UsageByCustomerDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsagesGroupByCustomer(
        DateTime FromDate,
        DateTime ToDate)
    {
        try
        {
            var groupedUsages = await _context.Usages
                .Where(_ => _.Date >= DateOnly.FromDateTime(FromDate) && _.Date <= DateOnly.FromDateTime(ToDate))
                .GroupBy(usage => usage.CustomerId)
                .Select(group => new UsageByCustomerDTO
                {
                    CustomerId = group.Key,
                    CustomerName = group.FirstOrDefault()!.CustomerName,
                    SimCount = group.Count(),
                    TotalUsage = group.Sum(usage => usage.TotalQuota),
                    LastUpdateInDays = (DateOnly.FromDateTime(DateTime.Now).DayNumber - group.Max(usage => usage.Date).DayNumber)
                })
                .ToListAsync();

            return Ok(groupedUsages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Usage group by customer");
            return BadRequest();
        }
    }
}

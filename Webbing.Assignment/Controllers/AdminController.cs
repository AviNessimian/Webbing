///
/// DO NOT EDIT !!
/// EXISTS ONLY FOR DB SEED 
/// SO YOU CAN RETREIVE AND TEST YOUR ENDPOINTS
/// 

namespace Webbing.Assignment.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context) => _context = context;

    [HttpGet("sims")]
    [ProducesResponseType(typeof(List<SimDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSims()
    {
        // DO NOT EDIT !

        var sims = await _context.Sims
            .Select(sim => new SimDTO
            {
                Id = sim.Id,
                CustomerId = sim.CustomerId
            })
            .ToListAsync();

        return Ok(sims);
    }

    [HttpGet("customers")]
    [ProducesResponseType(typeof(List<CustomerDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomers()
    {
        // DO NOT EDIT !!

        var customers = await _context.Customers
            .Select(customer => new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name
            })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("network-events")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNetworkEvents()
    {
        // DO NOT EDIT !!
        var networkEvents = await _context.NetworkEvents
            .Take(1000)
            .Select(networkEvent => new
            {
                networkEvent.SimId,
                networkEvent.SessionId,
                networkEvent.Quota,
                networkEvent.CreatedOnUtc
            })
            .ToListAsync();

        return Ok(networkEvents);
    }

    [HttpGet("usages")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsages()
    {
        var usages = await _context.Usages
            .Take(1000)
            .ToListAsync();

        return Ok(usages);
    }
    
}
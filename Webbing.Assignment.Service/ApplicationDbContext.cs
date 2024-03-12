using Webbing.Assignment.Service.Entities;

namespace Webbing.Assignment.Service;

public class ApplicationDbContext : DbContext
{
    private static bool _created = false;

    private static readonly Guid[] _guids = Enumerable.Range(1, 100).Select(i => Guid.NewGuid()).ToArray();

    // TO BE USE AS A QUEUE !!
    public DbSet<NetworkEvent> NetworkEvents { get; set; }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Sim> Sims { get; set; }

    public DbSet<Usage> Usages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        if (!_created)
        {
            Seed();
            _created = true;
        }
    }

    private void Seed()
    {
        var random = new Random(42);

        for (int i = 0; i < 100; i++)
        {
            var customer = new Customer { Id = Guid.NewGuid(), Name = $"Customer-{i}" };

            Customers.Add(customer);
        }
        SaveChanges();

        for (int i = 0; i < 20; i++)
        {
            var customer = Customers.Skip(random.Next(1, 99)).First();
            var sim = new Sim { Id = Guid.NewGuid(), CustomerId = customer.Id };

            Sims.Add(sim);
        }
        SaveChanges();

        for (int i = 0; i < 10000; i++)
        {
            var sim = Sims.Skip(random.Next(1, 19)).First();
            var networkEvent = new NetworkEvent
            {
                Id = Guid.NewGuid(),
                SessionId = _guids.Skip(random.Next(1, 99)).FirstOrDefault(),
                SimId = sim.Id,
                CreatedOnUtc = DateTime.UtcNow,
                Quota = random.Next(99999, 999999),
            };

            NetworkEvents.Add(networkEvent);
        }
        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Sim>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<NetworkEvent>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Usage>()
                   .HasKey(e => new { e.SimId, e.CustomerId, e.Date });

        base.OnModelCreating(modelBuilder);
    }
}
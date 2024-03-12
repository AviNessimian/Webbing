namespace Webbing.Assignment.Service.Consumers
{
    public class NetworkEventProcessor
    {
        private readonly ILogger<NetworkEventProcessor> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _thresholdTimeInHours;
        private readonly int _partitions;
        private readonly int _processLimit;

        public NetworkEventProcessor(
            ILogger<NetworkEventProcessor> logger,
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _thresholdTimeInHours = configuration.GetValue("NetworkEvent:Consumers:ThresholdTimeInHours", 24);
            _partitions = configuration.GetValue("NetworkEvent:Consumers:Partitions", 1);
            _processLimit = configuration.GetValue("NetworkEvent:Consumers:ProcessLimit", 200);
        }

        public DateTime LastEventCreatedOnUtc { get; set; } = DateTime.MinValue;

        public async Task Process(int partitionIndex)
        {
            if (partitionIndex < 0)
            {
                _logger.LogWarning("Partition key most be positive number");
                return;
            }

            try
            {
                _logger.LogDebug("Processing events, {PartitionIndex}", partitionIndex);

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var thresholdTime = DateTime.Now.AddHours(-_thresholdTimeInHours);
                var eventsToProcess = dbContext.NetworkEvents
                    .Where(_ => _.CreatedOnUtc >= thresholdTime && _.CreatedOnUtc > LastEventCreatedOnUtc)
                    .OrderBy(_ => _.CreatedOnUtc)
                    .Take(_processLimit);

                foreach (var @event in eventsToProcess)
                {
                    if (@event.GetPartition(_partitions) != partitionIndex) continue;

                    _logger.LogDebug("Processing event, {EventId}", @event.Id);

                    var sim = await dbContext.Sims.SingleOrDefaultAsync(_ => _.Id == @event.SimId);
                    if (sim == null)
                    {
                        _logger.LogWarning("Sim not found, {SimId}", @event.SimId);
                        continue;
                    }

                    var usage = await dbContext.Usages.SingleOrDefaultAsync(_ => 
                        _.SimId == sim.Id  
                        && _.CustomerId == sim.CustomerId 
                        && _.Date == DateOnly.FromDateTime(@event.CreatedOnUtc));

                    if (usage != null)
                    {
                        _logger.LogDebug("Updating usage, {CustomerId}, {SimId}", sim.CustomerId, sim.Id);
                        usage.TotalQuota = @event.Quota;
                    }
                    else
                    {
                        var customer = await dbContext.Customers.SingleOrDefaultAsync(_ => _.Id == sim.CustomerId);
                        if (customer == null)
                        {
                            _logger.LogWarning("Customer not found, {CustomerId}", sim.CustomerId);
                            continue;
                        }

                        _logger.LogInformation("Creating new usage, {CustomerId}, {SimId}", sim.CustomerId, sim.Id);
                        dbContext.Usages.Add(new Entities.Usage
                        {
                            CustomerId = sim.CustomerId,
                            SimId = sim.Id,
                            CustomerName = customer.Name,
                            TotalQuota = @event.Quota,
                            Date = DateOnly.FromDateTime(@event.CreatedOnUtc)
                        });
                    }

                    _logger.LogInformation("Updating offset, {LastEventCreatedOnUtc}", @event.CreatedOnUtc);
                    LastEventCreatedOnUtc = @event.CreatedOnUtc;

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing events, {PartitionIndex}", partitionIndex);
            }
        }
    }
}

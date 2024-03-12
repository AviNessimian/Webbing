namespace Webbing.Assignment.Service.Consumers
{
    public class NetworkEventConsumer : BackgroundService
    {
        private readonly ILogger<NetworkEventConsumer> _logger;
        private readonly NetworkEventProcessor _dataProcessor;
        private readonly TimeSpan _periodTimeSpan;
        private int _partitionIndex;

        public NetworkEventConsumer(
            ILogger<NetworkEventConsumer> logger,
            IConfiguration configuration,
            NetworkEventProcessor dataProcessor)
        {
            _logger = logger;
            _partitionIndex = configuration.GetValue("NetworkEvent:Consumers:PartitionIndex", 0);
            var periodInSeconds = configuration.GetValue("NetworkEvent:Consumers:PeriodInMilliseconds", 100);
            _periodTimeSpan = TimeSpan.FromMilliseconds(periodInSeconds);
            _dataProcessor = dataProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_periodTimeSpan, stoppingToken);
                    _logger.LogDebug("Process data, {PartitionIndex}", _partitionIndex);
                    await _dataProcessor.Process(_partitionIndex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Execute error, {PartitionIndex}", _partitionIndex);
                }
            }
        }
    }
}

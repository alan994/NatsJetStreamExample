using Nats.Utils;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using System.Text;

namespace Consumer1.Web
{
    public class ConsumerSyncB : BackgroundService
    {
        private ConnectionFactory? natsConnFactory;
        private IConnection? natsConn;
        private IJetStreamPushSyncSubscription? subscription;
        private IJetStream? jsContext;
        private readonly ILogger<ConsumerSyncB> logger;

        public ConsumerSyncB(ILogger<ConsumerSyncB> logger)
        {
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.natsConnFactory = new ConnectionFactory();


            this.natsConn = natsConnFactory.CreateConnection(Utils.GetOptions());

            this.jsContext = natsConn.CreateJetStreamContext();

            var consumerConfiguration = ConsumerConfiguration.Builder()
                                   .WithDurable("natsconsumersync1")
                                    .WithDeliverSubject("something-else")
                                    .WithDeliverGroup("message-senders")
                                   .Build();

            var jsm = natsConn.CreateJetStreamManagementContext();

            jsm.AddOrUpdateConsumer(Utils.StreamName, consumerConfiguration);

            var pushConsumerOptions = PushSubscribeOptions.BindTo(Utils.StreamName, "natsconsumersync1");

            this.subscription = jsContext.PushSubscribeSync("send.sms", "message-senders", pushConsumerOptions);

            while (true)
            {
                try
                {
                    Msg msg = this.subscription.NextMessage();
                    var message = Encoding.UTF8.GetString(msg.Data);
                    this.logger.LogInformation("MESSAGE: " + message);
                    //this.logger.LogInformation("METADATA: " + JsonConvert.SerializeObject(msg.MetaData));
                    await Task.Delay(2000);
                    msg.Ack();
                }
                catch (NATSTimeoutException)
                {
                    // timeout is acceptable, means no messages available.
                }
            }
               
            this.logger.LogInformation(subscription.Stream);
            //subscription.Start();

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this.subscription?.Dispose();
            this.natsConn?.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}

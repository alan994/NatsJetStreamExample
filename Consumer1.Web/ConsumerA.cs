using Nats.Utils;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using System.Text;

namespace Consumer1.Web
{
    public class ConsumerA : BackgroundService
    {
        private ConnectionFactory? natsConnFactory;
        private IConnection? natsConn;
        private IJetStreamPushAsyncSubscription? subscription;
        private IJetStream? jsContext;
        private readonly ILogger<ConsumerA> logger;

        public ConsumerA(ILogger<ConsumerA> logger)
        {
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.natsConnFactory = new ConnectionFactory();


            this.natsConn = natsConnFactory.CreateConnection(Utils.GetOptions());

            this.jsContext = natsConn.CreateJetStreamContext();

            var pushConsumerOptions = ConsumerConfiguration.Builder()
                                    .WithDurable("natsconsumer1")
                                    //.WithDeliverGroup("message-senders")
                                    .BuildPushSubscribeOptions();

            this.subscription = jsContext.PushSubscribeAsync("send.*", (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message.Data);
                this.logger.LogInformation("MESSAGE: " + message);
                this.logger.LogInformation("METADATA: " + JsonConvert.SerializeObject(args.Message.MetaData));

                args.Message.Ack();
            }, false, pushConsumerOptions);


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

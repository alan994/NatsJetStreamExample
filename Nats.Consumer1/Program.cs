using Nats.Utils;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using System.Text;

var natsConnFactory = new ConnectionFactory();


using (var natsConn = natsConnFactory.CreateConnection(Utils.GetOptions()))
{
    var jsContext = natsConn.CreateJetStreamContext();
    IJetStreamManagement jsm = natsConn.CreateJetStreamManagementContext();

    var option = 2;

    if (option == 1)
    {
        var pushConsumerOptions = ConsumerConfiguration.Builder()
                                .WithDurable("natsconsumer1")
                                .WithDeliverGroup("message-senders")
                                .BuildPushSubscribeOptions();

        var subscription = jsContext.PushSubscribeAsync("send.*", (object sender, MsgHandlerEventArgs args) =>
        {
            var message = Encoding.UTF8.GetString(args.Message.Data);
            Console.WriteLine("MESSAGE: " + message);
            Console.WriteLine("METADATA: ", JsonConvert.SerializeObject(args.Message.MetaData));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            args.Message.Ack();
        }, false, pushConsumerOptions);


        Console.WriteLine(subscription.Stream);
        subscription.Start();
        Console.ReadLine();
    }
    else
    {
        var consumerOptions = ConsumerConfiguration.Builder()
                                .WithDurable("natsconsumer1")
                                .Build();

        var pullConsumerOptions = PullSubscribeOptions.Builder()
                                    .WithConfiguration(consumerOptions)                                    
                                    .WithStream(Utils.StreamName)
                                    .Build();

        using (var consumer = jsContext.PullSubscribe("send.mail", pullConsumerOptions))
        {
            natsConn.Flush(1000);

            while (true)
            {
                var message = consumer.NextMessage();
                
                Console.WriteLine("MESSAGE: " + Encoding.UTF8.GetString(message.Data));                
                Console.WriteLine("META DATA: " + JsonConvert.SerializeObject(message.MetaData));
                message.Ack();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
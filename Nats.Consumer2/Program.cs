using Nats.Utils;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using System.Text;



var natsConnFactory = new ConnectionFactory();


using (var natsConn = natsConnFactory.CreateConnection(Utils.GetOptions()))
{
    //Utils.EnsureStream(natsConn);

    var jsContext = natsConn.CreateJetStreamContext();
    IJetStreamManagement jsm = natsConn.CreateJetStreamManagementContext();







    

    

    var option = 1;
    if (option == 1)
    {
        var consumerConfiguration = ConsumerConfiguration.Builder()
                                .WithDurable("natsconsumer2")
                                .WithDeliverGroup("message-senders")
                                .Build();
        var pushConsumerOptions = PushSubscribeOptions.Builder()
                                .WithStream(Utils.StreamName)                                
                                .WithConfiguration(consumerConfiguration)
                                .Build();

        

        var subscription = jsContext.PushSubscribeAsync("send.*", (object sender, MsgHandlerEventArgs args) =>
        {
            var message = Encoding.UTF8.GetString(args.Message.Data);
            Console.WriteLine("MESSAGE: " + message);
            Console.WriteLine("METADATA: ", JsonConvert.SerializeObject(args.Message.MetaData));

            global::System.Console.WriteLine();
            global::System.Console.WriteLine();
            global::System.Console.WriteLine();
            args.Message.Ack();
        }, false, pushConsumerOptions);


        subscription.Start();
        Console.ReadLine();
    }

    else
    {
        var pullConsumerOptions = ConsumerConfiguration.Builder()
                                .WithDurable("natsconsumer2")                                
                                .BuildPullSubscribeOptions();

        using (var consumer = jsContext.PullSubscribe("send.*", pullConsumerOptions))
        {


            while (true)
            {
                var message = consumer.NextMessage();
                Console.WriteLine("-------------------- MESSAGE ----------------------------");
                Console.WriteLine(JsonConvert.SerializeObject(Encoding.UTF8.GetString(message.Data)));
                Console.WriteLine("----------------------- META DATA -------------------------");
                Console.WriteLine(JsonConvert.SerializeObject(message.MetaData));
                message.Ack();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
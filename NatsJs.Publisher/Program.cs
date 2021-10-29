using Nats.Utils;
using NATS.Client;
using NATS.Client.JetStream;
using Newtonsoft.Json;
using System.Text;
using static NATS.Client.JetStream.ConsumerConfiguration;

var natsConnFactory = new ConnectionFactory();

using (var natsConn = natsConnFactory.CreateConnection(Utils.GetOptions()))
{
    Utils.EnsureStream(natsConn);

    var jsContext = natsConn.CreateJetStreamContext();

    while (true)
    {
        var message = new Msg(
            "send.mail", 
            null, 
            new MsgHeader(), 
            Encoding.UTF8.GetBytes("Message " + Guid.NewGuid())
        );

        var ack = await jsContext.PublishAsync(message);
        if (ack.HasError)
        {
            Console.WriteLine($"ERROR: {ack.ErrorCode}:{ack.Error}");
        }
        else
        {
            Console.WriteLine("SUCCESS: " + JsonConvert.SerializeObject(ack));
            Console.WriteLine();
            Console.WriteLine();
        }
        await Task.Delay(1000);
    }
}
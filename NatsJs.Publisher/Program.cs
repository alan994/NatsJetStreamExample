using Nats.Utils;
using NATS.Client;
using Newtonsoft.Json;
using System.Text;

var natsConnFactory = new ConnectionFactory();

using (var natsConn = natsConnFactory.CreateConnection(Utils.GetOptions()))
{
    Utils.EnsureStream(natsConn);

    var jsContext = natsConn.CreateJetStreamContext();

    while (true)
    {
        var id = Guid.NewGuid();
        var message = new Msg(
            "send.mail", 
            null, 
            new MsgHeader(), 
            Encoding.UTF8.GetBytes("Message " + id)
        );

        var ack = await jsContext.PublishAsync(message);
        if (ack.HasError)
        {
            Console.WriteLine($"ERROR: {ack.ErrorCode}:{ack.Error}");
        }
        else
        {
            Console.WriteLine("SUCCESS: " + id);
            Console.WriteLine();
            Console.WriteLine();
        }
        await Task.Delay(1000);
        break;
    }
}
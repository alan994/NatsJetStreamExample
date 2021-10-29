using NATS.Client;
using NATS.Client.JetStream;

namespace Nats.Utils
{
    public static class Utils
    {
        public static string StreamName = "conversations";
        public static void EnsureStream(IConnection natsConn)
        {
            var jsStreamManagement = natsConn.CreateJetStreamManagementContext();

            try
            {
                jsStreamManagement.GetStreamInfo(StreamName);
            }
            catch (NATSJetStreamException ex)
            {
                var conversationStreamConfig = StreamConfiguration.Builder()
                        .WithName(StreamName)
                        .WithStorageType(StorageType.Memory)
                        .WithSubjects("send.*")
                        .Build();

                var stream = jsStreamManagement.AddStream(conversationStreamConfig);
                Console.WriteLine("STREAM: " + stream.ToString());
            }
            
        }

        public static Options GetOptions()
        {
            var options = ConnectionFactory.GetDefaultOptions();
            options.User = "alan";
            options.Password = "rotring123";

            return options;
        }
    }
}
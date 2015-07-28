using System;
using LanguageExt;
using static LanguageExt.Process;
using System.Threading;

namespace RedisStateSample
{
    class Program
    {
        //
        //  Launch a process that adds the int sent in a message to its state
        //  Then calls itself after a second to do the same again.  The state value gradually
        //  increases.  
        //
        //  If you stop the sample and restart you'll notice the state has been persisted
        //
        static void Main(string[] args)
        {
            // Log what's going on
            ProcessLog.Subscribe(Console.WriteLine);

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-test", "localhost", "0");

            // Spawn the process
            var pid = spawn<string>("redis-inbox-sample", Inbox, ProcessFlags.PersistInbox);

            Console.WriteLine("Press a key to add 100 messages to the process queue");
            Console.WriteLine(" - The process queue has a Thread.Sleep(200) in it");
            Console.WriteLine(" - So it takes 20 seconds to get through all of the messages");
            Console.WriteLine(" - If you quit the sample and restart, you should see the ");
            Console.WriteLine(" - inbox has persisted.");
            Console.WriteLine("");
            Console.WriteLine("Press a key");
            Console.ReadKey();

            var rnd = new Random();
            for (var i = 0; i < 100; i++)
            {
                tell(pid, "Message sent: " + DateTime.Now + " " + DateTime.Now.Ticks + " " + rnd.Next());
            }

            Console.ReadKey();
        }

        // 
        //  Inbox message handleer
        // 
        static void Inbox(string msg)
        {
            Console.WriteLine(msg);
            Thread.Sleep(200);
        }
    }
}

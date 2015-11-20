using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Process;

namespace RedisPublishSample
{
    class Program
    {
        //
        //  This sample sends messages published by a Language Ext process (by calling 'publish')
        //  to a Redis channel.  We then subscribe to the messages coming back where we push them
        //  out to the console.
        //
        static void Main(string[] args)
        {
            // Log what's going on
            // ProcessLog.Subscribe(Console.WriteLine);

            // Let Language Ext know that Redis exists
            RedisCluster.register();

            // Connect to the Redis cluster
            Cluster.connect("redis", "redis-publish-test", "localhost:6379", "0");

            // Launch a process that publishes a random number as fast as possible
            var pid = spawn<Random, int>("redis-pubsub-random-test", Setup, Inbox, ProcessFlags.RemotePublish);

            // Listen to the published results coming back from the Redis channel
            //subscribe<int>(pid, Console.WriteLine);

            // Start it off by sending the first message
            tell(pid, 0);

            Console.WriteLine("Values are being published.  Run RedisSubscribeSample to see them");
            Console.ReadKey();
        }

        //
        //  Get the initial state of the process
        //
        static Random Setup() => new Random();

        //
        //  Inbox message handleer
        //
        static Random Inbox(Random rnd, int value)
        {
            publish(value);
            tellSelf(rnd.Next());
            return rnd;
        }
    }
}

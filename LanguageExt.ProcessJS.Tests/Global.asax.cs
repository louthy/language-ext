using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt.ProcessJS.Tests
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RedisCluster.register();
        }

        static bool started = false;
        static object sync = new object();

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (!started)
            {
                lock (sync)
                {
                    if (!started)
                    {
                        ProcessConfig.initialiseWeb(() =>
                        {
                            started = true;

                            ProcessLog.startup(None);

                            // Ping-pong server process
                            spawn<string>("pingpong", PingPong.Inbox);

                            // Chat server process
                            spawn<string>("chat", msg => publish(msg));
                        });
                    }
                }
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
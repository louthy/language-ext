using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestBed
{
    public class BlockingQueue<T>
    {
        EventWaitHandle wait = new AutoResetEvent(true);
        Queue<T> queue = new Queue<T>();
        volatile bool paused;
        volatile bool cancelled;
        object sync = new object();

        public void Receive(Action<T> handler)
        {
            while (!cancelled)
            {
                if (queue.Count == 0)
                {
                    wait.WaitOne();
                }
                T[] items = null;
                lock (sync)
                {
                    items = new T[queue.Count];
                    queue.CopyTo(items, 0);
                    queue.Clear();
                }
                if (cancelled) return;
                foreach (var item in items)
                {
                    try
                    {
                        handler(item);
                        if (cancelled) return;
                        if (paused)
                        {
                            wait.WaitOne();
                        }
                    }
                    catch { }
                }
            }
        }

        public void Send(T value)
        {
            lock (sync)
            {
                queue.Enqueue(value);
            }
            if (!paused)
            {
                wait.Set();
            }
        }

        public void Cancel() =>
            cancelled = true;

        public void Pause() =>
            paused = true;

        public void UnPause()
        {
            paused = false;
            wait.Set();
        }
    }
}

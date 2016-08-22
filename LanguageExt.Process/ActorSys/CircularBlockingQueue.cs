using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt.ActorSys
{
    /// <summary>
    /// Lockless circular collection for implementing message queues
    /// </summary>
    internal class CircularBlockingQueue<T>
    {
        readonly EventWaitHandle wait = new AutoResetEvent(true);
        volatile bool paused;
        volatile bool cancelled;
        volatile int bufferHead = 0;
        volatile int bufferTail = 0;
        const int BufferMax = 1024;
        readonly T[] buffer;

        public readonly int Capacity;

        public CircularBlockingQueue(int capacity = BufferMax)
        {
            buffer = new T[capacity];
            Capacity = capacity;
        }

        public void Receive(Action<T> handler)
        {
            cancelled = false;
            paused = false;
            bufferHead = 0;
            bufferTail = 0;

            while (!cancelled)
            {
                if (bufferTail == bufferHead)
                {
                    wait.WaitOne();
                    if (cancelled) return;
                }
                while(bufferTail != bufferHead)
                {
                    if (cancelled) return;
                    try
                    {
                        handler(buffer[bufferTail]);
                        buffer[bufferTail] = default(T);
                    }
                    catch { }

                    bufferTail++;
                    if(bufferTail >= Capacity)
                    {
                        bufferTail = 0;
                    }

                    if (cancelled) return;
                    if (paused)
                    {
                        wait.WaitOne();
                    }
                }
            }
        }

        public int Count => bufferHead >= bufferTail
            ? bufferHead - bufferTail
            : Capacity - bufferTail + bufferHead;

        public void Post(T value)
        {
            if (Count < Capacity)
            {
                buffer[bufferHead] = value;
                bufferHead++;
                if (bufferHead >= Capacity)
                {
                    bufferHead = 0;
                }
                if (!paused)
                {
                    wait.Set();
                }
            }
            else
            {
                throw new Exception("Queue full");
            }
        }

        public void Cancel()
        {
            cancelled = true;
            wait.Set();
        }

        public void Pause() =>
            paused = true;

        public void UnPause()
        {
            paused = false;
            wait.Set();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    class ObjectPool<T>
    {
        readonly int initialSize;
        readonly object sync = new object();
        readonly Func<T> instantiator;
        readonly Stack<T> pool;

        public ObjectPool(int initialSize, Func<T> instantiator)
        {
            this.pool = new Stack<T>(initialSize);
            this.initialSize = initialSize;
            this.instantiator = instantiator;
        }

        private void Allocate()
        {
            for (var i = 0; i < initialSize; i++)
            {
                pool.Push(instantiator());
            }
        }

        public T GetItem()
        {
            lock(sync)
            {
                if (pool.Count == 0)
                {
                    Allocate();
                }
                var item = pool.Peek();
                pool.Pop();
                return item;
            }
        }

        public void Release(T item)
        {
            lock(sync)
            {
                pool.Push(item);
            }
        }
    }
}

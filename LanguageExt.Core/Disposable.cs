using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public interface ILinqDisposable : IDisposable
    {
    }

    public class LinqDisposable<T> : ILinqDisposable
        where T : class, IDisposable
    {
        public T Value { get; private set; }

        internal LinqDisposable(T value)
        {
            Value = value;
        }

        public void Dispose()
        {
            Value?.Dispose();
            Value = null;
        }
    }
}

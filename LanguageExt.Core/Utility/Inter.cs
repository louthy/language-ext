using System.Runtime.CompilerServices;
using System.Threading;

namespace LanguageExt
{
    internal static class Inter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int And(ref int loc, int value)
        {
            int current = loc;
            while (true)
            {
                int newValue = current & value;
                int oldValue = Interlocked.CompareExchange(ref loc, newValue, current);
                if (oldValue == current)
                {
                    return oldValue;
                }
                current = oldValue;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Represents a range of integers lazily.
    /// </summary>
#if !COREFX
    [Serializable]
#endif
    public class IntegerRange : IEnumerable<int>
    {
        public IntegerRange(int from, int count, int step = 1)
        {
            if (count < 0)
            {
                throw new ArgumentException("'count' should be zero or greater.");
            }
            From = from;
            Count = count;
            Step = step;
        }

        /// <summary>
        /// First integer in the range
        /// </summary>
        [Pure]
        public int From { get; }

        /// <summary>
        /// Count of integers in the range
        /// </summary>
        [Pure]
        public int Count { get; }

        /// <summary>
        /// Step size between integers
        /// </summary>
        [Pure]
        public int Step { get; }

        [Pure]
        public IEnumerable<int> AsEnumerable()
        {
            if (Count == 0) yield break;

            if (Step > 0)
            {
                var to = From + Count;
                if(to < From)
                {
                    throw new OverflowException();
                }
                for (var i = From; i < to; i += Step)
                {
                    yield return i;
                }
            }
            else
            {
                var to = From - Count;
                if (to > From)
                {
                    throw new OverflowException();
                }
                for (var i = From; i > to; i += Step)
                {
                    yield return i;
                }
            }
        }

        [Pure]
        public IEnumerator<int> GetEnumerator() => 
            AsEnumerable().GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => 
            AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Represents a range of chars lazily.
    /// </summary>
#if !COREFX
    [Serializable]
#endif
    public class CharRange : IEnumerable<char>
    {
        public CharRange(char from, char to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// First char in the range
        /// </summary>
        [Pure]
        public char From { get; }

        /// <summary>
        /// Last (and inclusive) in the range
        /// </summary>
        [Pure]
        public char To { get; }

        [Pure]
        public IEnumerable<char> AsEnumerable()
        {
            if (To > From)
            {
                for (var i = From; i <= To; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (var i = From; i >= To; i--)
                {
                    yield return i;
                }
            }
        }

        [Pure]
        public IEnumerator<char> GetEnumerator() => 
            AsEnumerable().GetEnumerator();

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() => 
            AsEnumerable().GetEnumerator();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// Represents a range of integers lazily.
    /// </summary>
    public class IntegerRange : IEnumerable<int>
    {
        public IntegerRange(int from, int count, int step = 1)
        {
            From = from;
            Count = count;
            Step = step;
        }

        /// <summary>
        /// First integer in the range
        /// </summary>
        public int From { get; }

        /// <summary>
        /// Count of integers in the range
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Step size between integers
        /// </summary>
        public int Step { get; }

        public IEnumerable<int> AsEnumerable()
        {
            var to = From + Count;
            for (var i = From; i < to; i += Step)
            {
                yield return i;
            }
        }

        public IEnumerator<int> GetEnumerator() => 
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Represents a range of chars lazily.
    /// </summary>
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
        public char From { get; }

        /// <summary>
        /// Last (and inclusive) in the range
        /// </summary>
        public char To { get; }

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

        public IEnumerator<char> GetEnumerator() => 
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            AsEnumerable().GetEnumerator();
    }

}

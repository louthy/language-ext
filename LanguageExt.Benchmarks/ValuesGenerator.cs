using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageExt.Benchmarks
{
    internal class ValuesGenerator
    {
        public static readonly ValuesGenerator Default = new ValuesGenerator(1234);

        readonly Random rand;

        public ValuesGenerator(int seed)
        {
            rand = new Random(seed);
        }

        public Dictionary<TKey, TValue> GenerateDictionary<TKey, TValue>(int count)
        {
            var dict = new Dictionary<TKey, TValue>(count);
            while (dict.Count < count)
            {
                dict[GenerateValue<TKey>()] = GenerateValue<TValue>();
            }

            return dict;
        }

        public T[] GenerateUniqueValues<T>(int count)
        {
            var set = new System.Collections.Generic.HashSet<T>(count);
            while (set.Count < count)
            {
                set.Add(GenerateValue<T>());
            }

            return set.ToArray();
        }

        public T GenerateValue<T>()
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)rand.Next();
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)GenerateString(1, 50);
            }

            throw new NotSupportedException($"Generating value of type {typeof(T)} is not supported");
        }

        private string GenerateString(int minLength, int maxLength)
        {
            var length = rand.Next(minLength, maxLength);
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                switch (rand.Next(0, 3))
                {
                    case 0:
                        sb.Append(rand.Next('0', '9'));
                        break;

                    case 1:
                        sb.Append(rand.Next('A', 'Z'));
                        break;

                    default:
                        sb.Append(rand.Next('a', 'z'));
                        break;
                }
            }

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageExt.Benchmarks
{
    internal partial class ValuesGenerator
    {
        public static readonly ValuesGenerator Default = new ValuesGenerator(12345);

        readonly int randSeed;

        public ValuesGenerator(int seed)
        {
            randSeed = seed;
        }

        public Dictionary<TKey, TValue> GenerateDictionary<TKey, TValue>(int count)
        {
            var rand = new Random(randSeed);

            var dict = new Dictionary<TKey, TValue>(count);
            while (dict.Count < count)
            {
                dict[GenerateValue<TKey>(rand)] = GenerateValue<TValue>(rand);
            }

            return dict;
        }

        public T[] GenerateUniqueValues<T>(int count)
        {
            var rand = new Random(randSeed);

            var set = new System.Collections.Generic.HashSet<T>(count);
            while (set.Count < count)
            {
                set.Add(GenerateValue<T>(rand));
            }

            return set.ToArray();
        }

        private T GenerateValue<T>(Random rand)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)rand.Next();
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)GenerateString(rand, 1, 50);
            }

            throw new NotSupportedException($"Generating value of type {typeof(T)} is not supported");
        }

        private string GenerateString(Random rand, int minLength, int maxLength)
        {
            var length = rand.Next(minLength, maxLength);
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                switch (rand.Next(0, 3))
                {
                    case 0:
                        sb.Append((char)rand.Next('0', '9'));
                        break;

                    case 1:
                        sb.Append((char)rand.Next('A', 'Z'));
                        break;

                    default:
                        sb.Append((char)rand.Next('a', 'z'));
                        break;
                }
            }

            return sb.ToString();
        }
    }
}

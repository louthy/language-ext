using System.Collections;
using System.Collections.Generic;
using Xunit;
using static LanguageExt.Prelude;
using LanguageExt;

namespace LanguageExt.Tests
{
    
    public class TryOutTests
    {
        [Fact]
        public void OutTest()
        {
            int value1 = parseInt("123").IfNone(() => 0);

            int value2 = ifNone(parseInt("123"), () => 0);

            int value3 = parseInt("123").IfNone(0);

            int value4 = ifNone(parseInt("123"), 0);

            Assert.True(value1 == 123);
            Assert.True(value2 == 123);
            Assert.True(value3 == 123);
            Assert.True(value4 == 123);

            parseInt("123").Match(
                Some: UseTheInteger,
                None: () => failwith<int>("Not an integer")
                );

            match( parseInt("123"),
                Some: UseTheInteger,
                None: () => failwith<int>("Not an integer")
                );

            int value5 = ifNone(parseInt("XXX"), 0);
            Assert.True(value5 == 0);
        }

        private int UseTheInteger(int v)
        {
            return 0;
        }

        void ShouldCompileTest()
        {
            var x = new DualDict<string, int>();
            var k = "";

            var y = x.TryGetValue(ReadOnlyKey: k);
        }

        class DualDict<K, V> : IDictionary<K, V>, IReadOnlyDictionary<K, V>
        {
            public V this[K key] => throw new System.NotImplementedException();

            V IDictionary<K, V>.this[K key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public IEnumerable<K> Keys => throw new System.NotImplementedException();

            public IEnumerable<V> Values => throw new System.NotImplementedException();

            public int Count => throw new System.NotImplementedException();

            public bool IsReadOnly => throw new System.NotImplementedException();

            ICollection<K> IDictionary<K, V>.Keys => throw new System.NotImplementedException();

            ICollection<V> IDictionary<K, V>.Values => throw new System.NotImplementedException();

            public void Add(K key, V value)
            {
                throw new System.NotImplementedException();
            }

            public void Add(KeyValuePair<K, V> item)
            {
                throw new System.NotImplementedException();
            }

            public void Clear()
            {
                throw new System.NotImplementedException();
            }

            public bool Contains(KeyValuePair<K, V> item)
            {
                throw new System.NotImplementedException();
            }

            public bool ContainsKey(K key)
            {
                throw new System.NotImplementedException();
            }

            public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
            {
                throw new System.NotImplementedException();
            }

            public bool Remove(K key)
            {
                throw new System.NotImplementedException();
            }

            public bool Remove(KeyValuePair<K, V> item)
            {
                throw new System.NotImplementedException();
            }

            public bool TryGetValue(K key, out V value)
            {
                value = default(V);
                return true;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

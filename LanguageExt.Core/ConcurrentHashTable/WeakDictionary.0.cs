/*  
 Copyright 2008 The 'A Concurrent Hashtable' development team  
 (http://www.codeplex.com/CH/People/ProjectPeople.aspx)

 This library is licensed under the GNU Library General Public License (LGPL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://www.codeplex.com/CH/license.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Security;

namespace TvdP.Collections
{
    /// <summary>
    /// Represents a thread-safe collection of key-value pairs that can be accessed
    /// by multiple threads concurrently and has weak references to the values.
    /// </summary>
    /// <typeparam name="TStrongKey">The type of the keys in this dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in this dictionary. This must be a reference type.</typeparam>
    /// <remarks>
    /// Whenever any of the values held by this dictionary is garbage collected the key-value pair holding the value will be removed from the dictionary.
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class WeakDictionary<TStrongKey, TValue> : DictionaryBase<TStrongKey, TValue>
#if !SILVERLIGHT
    , ISerializable
#endif
        where TValue : class
    {
        sealed class InternalWeakDictionary :
            InternalWeakDictionaryWeakValueBase<
                StrongKey<TStrongKey>, 
                TStrongKey, 
                TValue,
                StrongKey<TStrongKey>
            >
        {
            public InternalWeakDictionary(int concurrencyLevel, int capacity, KeyComparer<TStrongKey> keyComparer)
                : base(concurrencyLevel, capacity, keyComparer)
            { 
                _comparer = keyComparer;
                MaintenanceWorker.Register(this);
            }

            public InternalWeakDictionary(KeyComparer<TStrongKey> keyComparer)
                : base(keyComparer)
            { 
                _comparer = keyComparer;
                MaintenanceWorker.Register(this);
            }

            public KeyComparer<TStrongKey> _comparer;

            protected override StrongKey<TStrongKey> FromExternalKeyToSearchKey(TStrongKey externalKey)
            { return new StrongKey<TStrongKey>(externalKey); }

            protected override StrongKey<TStrongKey> FromExternalKeyToStorageKey(TStrongKey externalKey)
            { return new StrongKey<TStrongKey>(externalKey); }

            protected override StrongKey<TStrongKey> FromStackKeyToSearchKey(StrongKey<TStrongKey> externalKey)
            { return externalKey; }

            protected override StrongKey<TStrongKey> FromStackKeyToStorageKey(StrongKey<TStrongKey> externalKey)
            { return externalKey; }

            protected override bool FromInternalKeyToExternalKey(StrongKey<TStrongKey> internalKey, out TStrongKey externalKey)
            {
                externalKey = internalKey._element;
                return true; 
            }

            protected override bool FromInternalKeyToStackKey(StrongKey<TStrongKey> internalKey, out StrongKey<TStrongKey> externalKey)
            {
                externalKey = internalKey;
                return true; 
            }
        }

        readonly InternalWeakDictionary _internalDictionary;

        protected override IDictionary<TStrongKey, TValue> InternalDictionary
        { get { return _internalDictionary; } }

#if !SILVERLIGHT
        WeakDictionary(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            var comparer = (KeyComparer<TStrongKey>)serializationInfo.GetValue("Comparer", typeof(KeyComparer<TStrongKey>));
            var items = (List<KeyValuePair<TStrongKey, TValue>>)serializationInfo.GetValue("Items", typeof(List<KeyValuePair<TStrongKey, TValue>>));
            _internalDictionary = new InternalWeakDictionary(comparer);
            _internalDictionary.InsertContents(items);
        }

        #region ISerializable Members

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Comparer", _internalDictionary._comparer);
            info.AddValue("Items", _internalDictionary.GetContents());
        }
        #endregion
#endif

        public WeakDictionary()
            : this(EqualityComparer<TStrongKey>.Default)
        {}

        public WeakDictionary(IEqualityComparer<TStrongKey> strongKeyComparer)
            : this(Enumerable.Empty<KeyValuePair<TStrongKey,TValue>>(), strongKeyComparer)
        {}

        public WeakDictionary(IEnumerable<KeyValuePair<TStrongKey,TValue>> collection)
            : this(collection, EqualityComparer<TStrongKey>.Default)
        {}

        public WeakDictionary(IEnumerable<KeyValuePair<TStrongKey, TValue>> collection, IEqualityComparer<TStrongKey> strongKeyComparer)
        {
            _internalDictionary = 
                new InternalWeakDictionary(
                    new KeyComparer<TStrongKey>(strongKeyComparer)
                )
            ;

            _internalDictionary.InsertContents(collection);
        }

        public WeakDictionary(int concurrencyLevel, int capacity)
            : this(concurrencyLevel, capacity, EqualityComparer<TStrongKey>.Default)
        {}

        public WeakDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TStrongKey, TValue>> collection, IEqualityComparer<TStrongKey> strongKeyComparer)
        {
            var contentsList = collection.ToList();
            _internalDictionary =
                new InternalWeakDictionary(
                    concurrencyLevel,
                    contentsList.Count,
                    new KeyComparer<TStrongKey>(strongKeyComparer)
                )
            ;
            _internalDictionary.InsertContents(contentsList);
        }

        public WeakDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TStrongKey> strongKeyComparer)
        {
            _internalDictionary =
                new InternalWeakDictionary(
                    concurrencyLevel,
                    capacity,
                    new KeyComparer<TStrongKey>(strongKeyComparer)
                )
            ;
        }


        public bool ContainsKey(TStrongKey strongKey)
        { return _internalDictionary.ContainsKey( new StrongKey<TStrongKey>(strongKey) ); }

        public bool TryGetValue(TStrongKey strongKey, out TValue value)
        { return _internalDictionary.TryGetValue(new StrongKey<TStrongKey>(strongKey), out value); }

        public TValue this[TStrongKey strongKey]
        {
            get { return _internalDictionary.GetItem(new StrongKey<TStrongKey>(strongKey)); }
            set { _internalDictionary.SetItem(new StrongKey<TStrongKey>(strongKey), value); }
        }

        public bool IsEmpty
        { get { return _internalDictionary.IsEmpty; } }

        public TValue AddOrUpdate(TStrongKey strongKey, Func<TStrongKey, TValue> addValueFactory, Func<TStrongKey, TValue, TValue> updateValueFactory)
        {
            if (null == addValueFactory)
                throw new ArgumentNullException("addValueFactory");

            if (null == updateValueFactory)
                throw new ArgumentNullException("updateValueFactory");

            return
                _internalDictionary.AddOrUpdate(
                    new StrongKey<TStrongKey>(strongKey), 
                    hr => addValueFactory(hr._element), 
                    (hr, v) => updateValueFactory(hr._element, v)
                )
            ;
        }

        public TValue AddOrUpdate(TStrongKey strongKey, TValue addValue, Func<TStrongKey, TValue, TValue> updateValueFactory)
        {
            if (null == updateValueFactory)
                throw new ArgumentNullException("updateValueFactory");

            return
                _internalDictionary.AddOrUpdate(
                    new StrongKey<TStrongKey>(strongKey),
                    addValue,
                    (hr, v) => updateValueFactory(hr._element, v)
                )
            ;
        }

        public TValue GetOrAdd(TStrongKey strongKey, TValue value)
        { return _internalDictionary.GetOrAdd(new StrongKey<TStrongKey>(strongKey), value); }

        public TValue GetOrAdd(TStrongKey strongKey, Func<TStrongKey, TValue> valueFactory)
        {
            if (null == valueFactory)
                throw new ArgumentNullException("valueFactory");

            return _internalDictionary.GetOrAdd(new StrongKey<TStrongKey>(strongKey), hr => valueFactory(hr._element)); 
        }
        
        public KeyValuePair<TStrongKey, TValue>[] ToArray()
        { return _internalDictionary.ToArray(); }

        public bool TryAdd(TStrongKey strongKey, TValue value)
        { return _internalDictionary.TryAdd(new StrongKey<TStrongKey>(strongKey), value); }

        public bool TryRemove(TStrongKey strongKey, out TValue value)
        { return _internalDictionary.TryRemove(new StrongKey<TStrongKey>(strongKey), out value); }

        public bool TryUpdate(TStrongKey strongKey, TValue newValue, TValue comparisonValue)
        { return _internalDictionary.TryUpdate(new StrongKey<TStrongKey>(strongKey), newValue, comparisonValue); }
    }
}

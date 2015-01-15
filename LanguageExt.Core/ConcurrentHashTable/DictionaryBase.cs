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

namespace TvdP.Collections
{
    public abstract class DictionaryBase<EK,EV> :  IDictionary<EK, EV>, ICollection<KeyValuePair<EK, EV>>, IEnumerable<KeyValuePair<EK, EV>>, IDictionary, ICollection, IEnumerable
    {
        internal DictionaryBase()
        { }

        protected abstract IDictionary<EK, EV> InternalDictionary { get; }

        #region IDictionary Members

        void IDictionary.Add(object key, object value)
        { ((IDictionary<EK, EV>)this).Add((EK)key, (EV)value); }

        void IDictionary.Clear()
        { ((IDictionary<EK, EV>)this).Clear(); }

        bool IDictionary.Contains(object key)
        { return ((IDictionary<EK, EV>)this).ContainsKey((EK)key); }

        class DictionaryEnumerator : IDictionaryEnumerator
        {
            public IEnumerator<KeyValuePair<EK, EV>> _source;

            #region IDictionaryEnumerator Members

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get 
                {
                    var current = _source.Current;
                    return new DictionaryEntry( current.Key, current.Value ); 
                }
            }

            object IDictionaryEnumerator.Key
            { get { return _source.Current.Key; } }

            object IDictionaryEnumerator.Value
            { get { return _source.Current.Value; } }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            { get { return ((IDictionaryEnumerator)this).Entry; } }

            bool IEnumerator.MoveNext()
            { return _source.MoveNext(); }

            void IEnumerator.Reset()
            { _source.Reset(); }

            #endregion
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        { return new DictionaryEnumerator { _source = ((IEnumerable<KeyValuePair<EK, EV>>)this).GetEnumerator() }; }

        bool IDictionary.IsFixedSize
        { get { return false; } }

        bool IDictionary.IsReadOnly
        { get { return false; } }

        ICollection IDictionary.Keys
        { get { return (ICollection)((IDictionary<EK, EV>)this).Keys; } }

        void IDictionary.Remove(object key)
        { ((IDictionary<EK, EV>)this).Remove((EK)key); }

        ICollection IDictionary.Values
        { get { return (ICollection)((IDictionary<EK, EV>)this).Values; } }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary<EK, EV>)this)[(EK)key]; }
            set { ((IDictionary<EK, EV>)this)[(EK)key] = (EV)value; }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        { ((ICollection<KeyValuePair<EK,EV>>)this).CopyTo((KeyValuePair<EK,EV>[])array, index); }

        int ICollection.Count
        { get { return ((ICollection<KeyValuePair<EK,EV>>)this).Count; } }

        bool ICollection.IsSynchronized
        { get { return true; } }

        object ICollection.SyncRoot
        { get { return this; } }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        { return ((IEnumerable<KeyValuePair<EK,EV>>)this).GetEnumerator(); }

        #endregion

        #region IDictionary<EK,EV> Members

        void IDictionary<EK, EV>.Add(EK key, EV value)
        { InternalDictionary.Add(key, value); }

        bool IDictionary<EK, EV>.ContainsKey(EK key)
        { return InternalDictionary.ContainsKey(key); }

        ICollection<EK> IDictionary<EK, EV>.Keys
        { get { return InternalDictionary.Keys; } }

        bool IDictionary<EK, EV>.Remove(EK key)
        { return InternalDictionary.Remove(key); }

        bool IDictionary<EK, EV>.TryGetValue(EK key, out EV value)
        { return InternalDictionary.TryGetValue(key, out value); }

        ICollection<EV> IDictionary<EK, EV>.Values
        { get { return InternalDictionary.Values; } }

        EV IDictionary<EK, EV>.this[EK key]
        {
            get { return InternalDictionary[key]; }
            set { InternalDictionary[key] = value; }
        }

        #endregion

        #region ICollection<KeyValuePair<EK,EV>> Members

        void ICollection<KeyValuePair<EK, EV>>.Add(KeyValuePair<EK, EV> item)
        { InternalDictionary.Add(item); }

        void ICollection<KeyValuePair<EK, EV>>.Clear()
        { InternalDictionary.Clear(); }

        bool ICollection<KeyValuePair<EK, EV>>.Contains(KeyValuePair<EK, EV> item)
        { return InternalDictionary.Contains(item); }

        void ICollection<KeyValuePair<EK, EV>>.CopyTo(KeyValuePair<EK, EV>[] array, int arrayIndex)
        { InternalDictionary.CopyTo(array, arrayIndex); }

        int ICollection<KeyValuePair<EK, EV>>.Count
        { get { return InternalDictionary.Count; } }

        bool ICollection<KeyValuePair<EK, EV>>.IsReadOnly
        { get { return InternalDictionary.IsReadOnly; } }

        bool ICollection<KeyValuePair<EK, EV>>.Remove(KeyValuePair<EK, EV> item)
        { return InternalDictionary.Remove(item); }

        #endregion

        #region IEnumerable<KeyValuePair<EK,EV>> Members

        IEnumerator<KeyValuePair<EK, EV>> IEnumerable<KeyValuePair<EK, EV>>.GetEnumerator()
        { return ((IEnumerable<KeyValuePair<EK, EV>>)InternalDictionary).GetEnumerator(); }

        #endregion
    }
}

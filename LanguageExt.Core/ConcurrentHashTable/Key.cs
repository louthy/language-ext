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

namespace TvdP.Collections
{
    internal interface ITrashable
    { 
        bool IsGarbage { get; }
    };

    internal struct WeakKey<E> : ITrashable
        where E : class
    {
        public static object NullValue = new object();

        public object _elementReference;

        #region ITrashable Members

        public bool IsGarbage
        { get { return ((WeakReference)_elementReference).Target == null; } }

        public bool GetValue(out E value, bool isWeak)
        {
            object obj = isWeak ? ((WeakReference)_elementReference).Target : _elementReference;

            if (obj == null)
            {
                value = default(E);
                return false;
            }

            value = (E)( object.ReferenceEquals(NullValue,obj) ? null : obj );

            return true;
        }

        public void SetValue(E value, bool isWeak)
        {
            var obj = (object)value ?? NullValue;
            _elementReference = isWeak ? (object)(new WeakReference(obj)) : obj;
        }

        #endregion
    }

    internal struct StrongKey<E> : ITrashable
    {
        public StrongKey(E value)
        { _element = value; }

        public E _element;

        #region ITrashable Members

        public bool IsGarbage
        { get { return false; } }

        #endregion
    }

    internal struct KeySegment<E, T> : ITrashable
        where E : class
        where T : ITrashable
    {
        public WeakKey<E> _elementReference;
        public T _tail;

        #region ITrashable Members

        public bool IsGarbage
        { get { return _elementReference.IsGarbage || _tail.IsGarbage; } }

        public bool GetValue(out E value, bool isWeak)
        { return _elementReference.GetValue(out value, isWeak); }

        public void SetValue(E value, bool isWeak)
        { _elementReference.SetValue(value, isWeak); }

        #endregion
    }


    internal abstract class KeyBase<V, S> : ITrashable
        where V : ITrashable
    {
        public V _values;
        public S _strongValue;
        public int _hash;
        public abstract bool IsWeak { get; }

        #region ITrashable Members

        bool ITrashable.IsGarbage
        { get { return _values.IsGarbage; } }

        #endregion
    }


    internal abstract class Key<W1, S> : KeyBase<WeakKey<W1>, S>
        where W1 : class
    {
        public Key<W1, S> Set(Stacktype<W1, S> h, KeyComparer<W1, S> comparer)
        {
            bool isWeak = IsWeak;
            _values.SetValue(h.Item1, isWeak);
            _strongValue = h.Item2;
            _hash = comparer.CalculateHashCode(this);
            return this;
        }

        public Key<W1, S> Set(Tuple<W1, S> t, KeyComparer<W1, S> comparer)
        { return Set(t.AsStacktype(), comparer); }

        public bool Get(out Stacktype<W1, S> t)
        {
            t = new Stacktype<W1, S> { Item2 = _strongValue };
            bool res = _values.GetValue(out t.Item1, IsWeak);
            return res;
        }

        public bool Get(out Tuple<W1, S> t)
        {
            Stacktype<W1, S> h;
            bool res = Get(out h);
            t = h.AsTuple();
            return res;
        }
    }

    internal abstract class Key<W1, W2, S> : KeyBase<KeySegment<W1, WeakKey<W2>>, S>
        where W1 : class
        where W2 : class
    {
        public Key<W1, W2, S> Set(Stacktype<W1, W2, S> h, KeyComparer<W1, W2, S> comparer)
        {
            bool isWeak = IsWeak;
            _values.SetValue(h.Item1, isWeak);
            _values._tail.SetValue(h.Item2, isWeak);
            _strongValue = h.Item3;
            _hash = comparer.CalculateHashCode(this);
            return this;
        }

        public Key<W1, W2, S> Set(Tuple<W1, W2, S> t, KeyComparer<W1, W2, S> comparer)
        { return Set(t.AsStacktype(), comparer); }

        public bool Get(out Stacktype<W1, W2, S> t)
        {
            t = new Stacktype<W1, W2, S>() { Item3 = _strongValue };
            bool isWeak = IsWeak;
            bool res = _values.GetValue(out t.Item1, isWeak);
            res = _values._tail.GetValue(out t.Item2, isWeak) && res;
            return res;
        }

        public bool Get(out Tuple<W1, W2, S> t)
        {
            Stacktype<W1, W2, S> h;
            bool res = Get(out h);
            t = h.AsTuple();
            return res;
        }
    }

    internal abstract class Key<W1, W2, W3, S> : KeyBase<KeySegment<W1, KeySegment<W2, WeakKey<W3>>>, S>
        where W1 : class
        where W2 : class
        where W3 : class
    {
        public Key<W1, W2, W3, S> Set(Stacktype<W1, W2, W3, S> h, KeyComparer<W1, W2, W3, S> comparer)
        {
            bool isWeak = IsWeak;
            _values.SetValue(h.Item1, isWeak);
            _values._tail.SetValue(h.Item2, isWeak);
            _values._tail._tail.SetValue(h.Item3, isWeak);
            _strongValue = h.Item4;
            _hash = comparer.CalculateHashCode(this);
            return this;
        }

        public Key<W1, W2, W3, S> Set(Tuple<W1, W2, W3, S> t, KeyComparer<W1, W2, W3, S> comparer)
        { return Set(t.AsStacktype(), comparer); }

        public bool Get(out Stacktype<W1, W2, W3, S> t)
        {
            t = new Stacktype<W1, W2, W3, S> { Item4 = _strongValue };
            bool isWeak = IsWeak;
            bool res = _values.GetValue(out t.Item1, isWeak);
            res = _values._tail.GetValue(out t.Item2, isWeak) && res;
            res = _values._tail._tail.GetValue(out t.Item3, isWeak) && res;
            return res;
        }

        public bool Get(out Tuple<W1, W2, W3, S> t)
        {
            Stacktype<W1, W2, W3, S> h;
            bool res = Get(out h);
            t = h.AsTuple();
            return res;
        }
    }

    internal abstract class Key<W1, W2, W3, W4, S> : KeyBase<KeySegment<W1, KeySegment<W2, KeySegment<W3, WeakKey<W4>>>>, S>
        where W1 : class
        where W2 : class
        where W3 : class
        where W4 : class
    {
        public Key<W1, W2, W3, W4, S> Set(Stacktype<W1, W2, W3, W4, S> h, KeyComparer<W1, W2, W3, W4, S> comparer)
        {
            bool isWeak = IsWeak;
            _values.SetValue(h.Item1, isWeak);
            _values._tail.SetValue(h.Item2, isWeak);
            _values._tail._tail.SetValue(h.Item3, isWeak);
            _values._tail._tail._tail.SetValue(h.Item4, isWeak);
            _strongValue = h.Item5;
            _hash = comparer.CalculateHashCode(this);
            return this;
        }

        public Key<W1, W2, W3, W4, S> Set(Tuple<W1, W2, W3, W4, S> t, KeyComparer<W1, W2, W3, W4, S> comparer)
        { return Set(t.AsStacktype(), comparer); }

        public bool Get(out Stacktype<W1, W2, W3, W4, S> t)
        {
            t = new Stacktype<W1, W2, W3, W4, S> { Item5 = _strongValue };
            bool isWeak = IsWeak;
            bool res = _values.GetValue(out t.Item1, isWeak);
            res = _values._tail.GetValue(out t.Item2, isWeak) && res;
            res = _values._tail._tail.GetValue(out t.Item3, isWeak) && res;
            res = _values._tail._tail._tail.GetValue(out t.Item4, isWeak) && res;
            return res;
        }

        public bool Get(out Tuple<W1, W2, W3, W4, S> t)
        {
            Stacktype<W1, W2, W3, W4, S> h;
            bool res = Get(out h);
            t = h.AsTuple();
            return res;
        }
    }


    internal class StorageKey<W1, S> : Key<W1, S>
        where W1 : class
    { public override bool IsWeak { get { return true; } } }

    internal class StorageKey<W1, W2, S> : Key<W1, W2, S>
        where W1 : class
        where W2 : class
    { public override bool IsWeak { get { return true; } } }

    internal class StorageKey<W1, W2, W3, S> : Key<W1, W2, W3, S>
        where W1 : class
        where W2 : class
        where W3 : class
    { public override bool IsWeak { get { return true; } } }

    internal class StorageKey<W1, W2, W3, W4, S> : Key<W1, W2, W3, W4, S>
        where W1 : class
        where W2 : class
        where W3 : class
        where W4 : class
    { public override bool IsWeak { get { return true; } } }


    internal class SearchKey<W1, S> : Key<W1, S>
        where W1 : class
    { public override bool IsWeak { get { return false; } } }

    internal class SearchKey<W1, W2, S> : Key<W1, W2, S>
        where W1 : class
        where W2 : class
    { public override bool IsWeak { get { return false; } } }

    internal class SearchKey<W1, W2, W3, S> : Key<W1, W2, W3, S>
        where W1 : class
        where W2 : class
        where W3 : class
    { public override bool IsWeak { get { return false; } } }

    internal class SearchKey<W1, W2, W3, W4, S> : Key<W1, W2, W3, W4, S>
        where W1 : class
        where W2 : class
        where W3 : class
        where W4 : class
    { public override bool IsWeak { get { return false; } } }

}

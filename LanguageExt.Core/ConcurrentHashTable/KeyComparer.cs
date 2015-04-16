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
    internal interface IKeyComparer<T>
    {
        bool Equals(ref T x, bool xIsWeak, ref T y, bool yIsWeak);
        int GetHashCode(ref T obj, bool objIsWeak);
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    internal struct WeakKeyComparer<E> : IKeyComparer<WeakKey<E>>
        where E : class
    {
        public IEqualityComparer<E> _equalityComparer;

        #region IKeyComparer<WeakKey<E>> Members

        public bool Equals(ref WeakKey<E> key1, bool key1IsWeak, ref WeakKey<E> key2, bool key2IsWeak)
        {
            var obj1 = key1IsWeak ? ((WeakReference)key1._elementReference).Target : key1._elementReference;
            var obj2 = key2IsWeak ? ((WeakReference)key2._elementReference).Target : key2._elementReference;

            return
                obj1 == null ? obj2 == null && object.ReferenceEquals(key1._elementReference, key2._elementReference) :
                obj2 == null ? false :
                object.ReferenceEquals(obj1, obj2) ? true :
                _equalityComparer.Equals((E)obj1, (E)obj2);
        }

        public int GetHashCode(ref WeakKey<E> obj, bool objIsWeak)
        {
            return _equalityComparer.GetHashCode((E)(objIsWeak ? ((WeakReference)obj._elementReference).Target : obj._elementReference));
        }

        #endregion
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    internal class StrongKeyComparer<E> : IKeyComparer<StrongKey<E>>
    {
        public IEqualityComparer<E> _equalityComparer = null;

        #region IKeyComparer<StrongKey<E>> Members

        public bool Equals(ref StrongKey<E> x, bool xIsWeak, ref StrongKey<E> y, bool yIsWeak)
        { return _equalityComparer.Equals(x._element, y._element); }

        public int GetHashCode(ref StrongKey<E> obj, bool objIsWeak)
        { return _equalityComparer.GetHashCode(obj._element); }

        #endregion
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    internal class KeySegmentComparer<E, T> : IKeyComparer<KeySegment<E, T>>
        where E : class
        where T : ITrashable
    {
        public WeakKeyComparer<E> _elementComparer;
        public IKeyComparer<T> _tail;

        #region IKeyComparer<WeakKeySegment<E,T>> Members

        public bool Equals(ref KeySegment<E, T> x, bool xIsWeak, ref KeySegment<E, T> y, bool yIsWeak)
        {
            return
                _elementComparer.Equals(ref x._elementReference, xIsWeak, ref y._elementReference, yIsWeak)
                && _tail.Equals(ref x._tail, xIsWeak, ref y._tail, yIsWeak)
            ;
        }

        public int GetHashCode(ref KeySegment<E, T> obj, bool objIsWeak)
        {
            return (int)Hasher.Rehash(_elementComparer.GetHashCode(ref obj._elementReference, objIsWeak)) ^ _tail.GetHashCode(ref obj._tail, objIsWeak);
        }

        #endregion
    }


    internal abstract class KeyComparerBase<K, V, S> : IEqualityComparer<K>
        where K : KeyBase<V, S>
        where V : ITrashable
    {
        public IKeyComparer<V> _comparers;
        public IEqualityComparer<S> _strongValueComparer;

        #region IEqualityComparer<Key<A,B,C,D>> Members

        public bool Equals(K x, K y)
        { return _comparers.Equals(ref x._values, x.IsWeak, ref y._values, y.IsWeak) && _strongValueComparer.Equals(x._strongValue, y._strongValue); }

        public int GetHashCode(K obj)
        { return obj._hash; }

        public int CalculateHashCode(K obj)
        { return (int)Hasher.Rehash(_comparers.GetHashCode(ref obj._values, obj.IsWeak)) ^ _strongValueComparer.GetHashCode(obj._strongValue); }

        #endregion
    }


#if !SILVERLIGHT
    [Serializable]
#endif
    internal class KeyComparer<S> : IEqualityComparer<StrongKey<S>>
    {
        IEqualityComparer<S> _strongValueComparer;

        public KeyComparer(IEqualityComparer<S> strongKeyComparer)
        {
            if (null == strongKeyComparer)
                throw new ArgumentNullException("strongKeyComparer");

            _strongValueComparer = strongKeyComparer;
        }

        #region IEqualityComparer<StrongKey<S>> Members

        public bool Equals(StrongKey<S> x, StrongKey<S> y)
        { return _strongValueComparer.Equals(x._element, y._element); }

        public int GetHashCode(StrongKey<S> obj)
        { return _strongValueComparer.GetHashCode(obj._element); }

        #endregion
    }


#if !SILVERLIGHT
    [Serializable]
#endif
    internal class KeyComparer<W1, S> : KeyComparerBase<Key<W1, S>, WeakKey<W1>, S>
        where W1 : class
    {
        public KeyComparer(IEqualityComparer<W1> weakKeyComparer, IEqualityComparer<S> strongKeyComparer)
        {
            if (null == weakKeyComparer)
                throw new ArgumentNullException("weakKeyComparer");

            if (null == strongKeyComparer)
                throw new ArgumentNullException("strongKeyComparer");

            _comparers = new WeakKeyComparer<W1> { _equalityComparer = weakKeyComparer };
            _strongValueComparer = strongKeyComparer;
        }
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    internal class KeyComparer<W1, W2, S> : KeyComparerBase<Key<W1, W2, S>, KeySegment<W1, WeakKey<W2>>, S>
        where W1 : class
        where W2 : class
    {
        public KeyComparer(IEqualityComparer<W1> weakKey1Comparer, IEqualityComparer<W2> weakKey2Comparer, IEqualityComparer<S> strongKeyComparer)
        {
            if (null == weakKey1Comparer)
                throw new ArgumentNullException("weakKey1Comparer");

            if (null == weakKey2Comparer)
                throw new ArgumentNullException("weakKey2Comparer");

            if (null == strongKeyComparer)
                throw new ArgumentNullException("strongKeyComparer");

            _comparers =
                new KeySegmentComparer<W1, WeakKey<W2>>
                {
                    _elementComparer = new WeakKeyComparer<W1> { _equalityComparer = weakKey1Comparer },
                    _tail = new WeakKeyComparer<W2> { _equalityComparer = weakKey2Comparer }
                }
            ;

            _strongValueComparer = strongKeyComparer;
        }
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    internal class KeyComparer<W1, W2, W3, S> : KeyComparerBase<Key<W1, W2, W3, S>, KeySegment<W1, KeySegment<W2, WeakKey<W3>>>, S>
        where W1 : class
        where W2 : class
        where W3 : class
    {
        public KeyComparer(IEqualityComparer<W1> weakKey1Comparer, IEqualityComparer<W2> weakKey2Comparer, IEqualityComparer<W3> weakKey3Comparer, IEqualityComparer<S> strongKeyComparer)
        {
            if (null == weakKey1Comparer)
                throw new ArgumentNullException("weakKey1Comparer");

            if (null == weakKey2Comparer)
                throw new ArgumentNullException("weakKey2Comparer");

            if (null == weakKey3Comparer)
                throw new ArgumentNullException("weakKey3Comparer");

            if (null == strongKeyComparer)
                throw new ArgumentNullException("strongKeyComparer");

            _comparers =
                new KeySegmentComparer<W1, KeySegment<W2, WeakKey<W3>>>
                {
                    _elementComparer = new WeakKeyComparer<W1> { _equalityComparer = weakKey1Comparer },
                    _tail = new KeySegmentComparer<W2, WeakKey<W3>>
                    {
                        _elementComparer = new WeakKeyComparer<W2> { _equalityComparer = weakKey2Comparer },
                        _tail = new WeakKeyComparer<W3> { _equalityComparer = weakKey3Comparer }
                    }
                }
            ;

            _strongValueComparer = strongKeyComparer;
        }
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    internal class KeyComparer<W1, W2, W3, W4, S> : KeyComparerBase<Key<W1, W2, W3, W4, S>, KeySegment<W1, KeySegment<W2, KeySegment<W3, WeakKey<W4>>>>, S>
        where W1 : class
        where W2 : class
        where W3 : class
        where W4 : class
    {
        public KeyComparer(IEqualityComparer<W1> weakKey1Comparer, IEqualityComparer<W2> weakKey2Comparer, IEqualityComparer<W3> weakKey3Comparer, IEqualityComparer<W4> weakKey4Comparer, IEqualityComparer<S> strongKeyComparer)
        {
            if (null == weakKey1Comparer)
                throw new ArgumentNullException("weakKey1Comparer");

            if (null == weakKey2Comparer)
                throw new ArgumentNullException("weakKey2Comparer");

            if (null == weakKey3Comparer)
                throw new ArgumentNullException("weakKey3Comparer");

            if (null == weakKey4Comparer)
                throw new ArgumentNullException("weakKey4Comparer");

            if (null == strongKeyComparer)
                throw new ArgumentNullException("strongKeyComparer");

            _comparers =
                new KeySegmentComparer<W1, KeySegment<W2, KeySegment<W3, WeakKey<W4>>>>
                {
                    _elementComparer = new WeakKeyComparer<W1> { _equalityComparer = weakKey1Comparer },
                    _tail = new KeySegmentComparer<W2, KeySegment<W3, WeakKey<W4>>>
                    {
                        _elementComparer = new WeakKeyComparer<W2> { _equalityComparer = weakKey2Comparer },
                        _tail = new KeySegmentComparer<W3, WeakKey<W4>>
                        {
                            _elementComparer = new WeakKeyComparer<W3> { _equalityComparer = weakKey3Comparer },
                            _tail = new WeakKeyComparer<W4> { _equalityComparer = weakKey4Comparer }
                        }
                    }
                }
            ;

            _strongValueComparer = strongKeyComparer;
        }
    }
}

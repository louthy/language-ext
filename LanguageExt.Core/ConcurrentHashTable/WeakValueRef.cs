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
    internal interface IWeakValueRef<V>
    {
        object Reference{ get; }
        bool IsGarbage { get; }
        bool GetValue(out V value);
    }

    internal struct WeakValueRef<V> : IWeakValueRef<V>, IEquatable<WeakValueRef<V>>
        where V : class
    {
        public static WeakValueRef<V> Create(V value)
        { return new WeakValueRef<V> { _valueReference = new WeakReference(value == null ? NullValue : value) }; }

        static object NullValue = new object();

        WeakReference _valueReference;

        public object Reference { get{ return _valueReference; } }

        public bool IsGarbage
        { get { return _valueReference.Target == null; } }

        public bool GetValue(out V value)
        {
            object vObj = _valueReference.Target;

            if (null == vObj)
            {
                value = default(V);
                return false;
            }

            value = (V)(object.ReferenceEquals(NullValue, vObj) ? null : vObj);
            return true;
        }

        #region IEquatable<WeakValueRef<V>> Members

        public bool Equals(WeakValueRef<V> other)
        {
            //assume weak
            if (object.ReferenceEquals(_valueReference, other._valueReference))
                return true;

            var thisObj = _valueReference.Target;
            var otherObj = other._valueReference.Target;

            if (thisObj == null || otherObj == null)
                return false;

            return EqualityComparer<V>.Default.Equals((V)thisObj, (V)otherObj);
        }

        public override bool Equals(object obj)
        {
            return obj is WeakValueRef<V> && this.Equals((WeakValueRef<V>)obj);
        }

        public override int GetHashCode()
        {
            return _valueReference.IsAlive
                ? _valueReference.Target.GetHashCode()
                : 0;
        }

        //no relyable hashcode.

        #endregion
    }
}

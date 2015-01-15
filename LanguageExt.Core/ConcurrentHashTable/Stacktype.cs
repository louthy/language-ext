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
    internal struct Stacktype<T1>
    {
        public T1 Item1;

        public Tuple<T1> AsTuple()
        { return Tuple.Create(Item1); }
    }
    internal struct Stacktype<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Tuple<T1, T2> AsTuple()
        { return Tuple.Create(Item1, Item2); }
    }

    internal struct Stacktype<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public Tuple<T1, T2, T3> AsTuple()
        { return Tuple.Create(Item1, Item2, Item3); }
    }

    internal struct Stacktype<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public Tuple<T1, T2, T3, T4> AsTuple()
        { return Tuple.Create(Item1, Item2, Item3, Item4); }
    }

    internal struct Stacktype<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public Tuple<T1, T2, T3, T4, T5> AsTuple()
        { return Tuple.Create(Item1, Item2, Item3, Item4, Item5); }
    }

    internal static class Stacktype
    {
        public static Stacktype<T1, T2> AsStacktype<T1, T2>(this Tuple<T1, T2> key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            return new Stacktype<T1, T2> { Item1 = key.Item1, Item2 = key.Item2 };
        }

        public static Stacktype<T1, T2, T3> AsStacktype<T1, T2, T3>(this Tuple<T1, T2, T3> key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            return new Stacktype<T1, T2, T3> { Item1 = key.Item1, Item2 = key.Item2, Item3 = key.Item3 };
        }

        public static Stacktype<T1, T2, T3, T4> AsStacktype<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            return new Stacktype<T1, T2, T3, T4> { Item1 = key.Item1, Item2 = key.Item2, Item3 = key.Item3, Item4 = key.Item4 };
        }

        public static Stacktype<T1, T2, T3, T4, T5> AsStacktype<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            return new Stacktype<T1, T2, T3, T4, T5> { Item1 = key.Item1, Item2 = key.Item2, Item3 = key.Item3, Item4 = key.Item4, Item5 = key.Item5 };
        }

        public static Stacktype<T1> Create<T1>(T1 t1)
        { return new Stacktype<T1> { Item1 = t1 }; }

        public static Stacktype<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        { return new Stacktype<T1, T2> { Item1 = t1, Item2 = t2 }; }

        public static Stacktype<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        { return new Stacktype<T1, T2, T3> { Item1 = t1, Item2 = t2, Item3 = t3 }; }

        public static Stacktype<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        { return new Stacktype<T1, T2, T3, T4> { Item1 = t1, Item2 = t2, Item3 = t3, Item4 = t4 }; }

        public static Stacktype<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        { return new Stacktype<T1, T2, T3, T4, T5> { Item1 = t1, Item2 = t2, Item3 = t3, Item4 = t4, Item5 = t5 }; }
    }


}

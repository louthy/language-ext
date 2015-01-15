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
    class TransformedCollection<TOut> : ICollection<TOut>, ICollection
    {
        public IEnumerable<TOut> _source;

        #region ICollection<TOut> Members

        public void Add(TOut item)
        { throw new NotImplementedException(); }

        public void Clear()
        { throw new NotImplementedException(); }

        public bool Contains(TOut item)
        { return _source.Contains(item); }

        public void CopyTo(TOut[] array, int arrayIndex)
        {
            foreach (var item in _source)
                array[arrayIndex++] = item;
        }

        public int Count
        { get { return _source.Count(); } }

        public bool IsReadOnly
        { get { return true; } }

        public bool Remove(TOut item)
        { throw new NotImplementedException(); }

        #endregion

        #region IEnumerable<TOut> Members

        public IEnumerator<TOut> GetEnumerator()
        { return _source.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        { return this.GetEnumerator(); }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        { this.CopyTo((TOut[])array, index); }

        int ICollection.Count
        { get { return this.Count; } }

        bool ICollection.IsSynchronized
        { get { return true; } }

        object ICollection.SyncRoot
        { get { return this; } }

        #endregion
    }
}

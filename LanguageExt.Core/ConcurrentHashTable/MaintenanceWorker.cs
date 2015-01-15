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
using System.Threading;

namespace TvdP.Collections
{
    /// <summary>
    /// Helper class for ConcurrentWeakHashtable. Makes sure that its DoMaintenance method
    /// gets called when the GarbageCollector has collected garbage.
    /// </summary>
    internal static class MaintenanceWorker
    {
        #region Garbage Collection tracking

        /// <summary>
        /// Check all table registrations for continued existence. Removes the entries for
        /// tables that have been GC'd
        /// </summary>
        private static void RemoveVoidTables()
        {
            //empty head may remain.. not bad.
            var pos = _TableList;

            if (pos != null)
                while (true)
                {
                    var next = pos.Next;

                    if (next == null)
                        break;

                    if (next.Target == null)
                        pos.Next = next.Next; //safe, only 1 thread doing this
                    else
                        pos = next;
                }
        }

        /// <summary>
        /// true if a table maintenance session is scheduled or ongoing.
        /// </summary>
        static int _TableMaintenanceIsPending = 0;

#if !SILVERLIGHT
        /// <summary>
        /// Small timer that will
        /// check every 1/10 second if second level GC count increased. 
        /// </summary>
        static Timer _Timer = new Timer(CheckGCCount, null, 100, 100);

        static int _Level2GCCount;
#else
        /// <summary>
        /// Small timer that will start a garbage sweep every 10 seconds.
        /// In growing hashtables the garbage will get swept regularly but they
        /// can not shrink without a sweep.
        /// </summary>
        static Timer _Timer = new Timer(CheckGCCount, null, 10000, 10000);
#endif


        /// <summary>
        /// Checks if a garbage collection has indeed taken place and schedules
        /// a table maintenance session.
        /// </summary>
        /// <param name="dummy"></param>
        static internal void CheckGCCount(object dummy)
        {
#if !SILVERLIGHT
            if (_Level2GCCount != GC.CollectionCount(2) && Interlocked.CompareExchange(ref _TableMaintenanceIsPending, 1, 0) == 0)
            {
                _Level2GCCount = GC.CollectionCount(2);
#else
            if (Interlocked.CompareExchange(ref _TableMaintenanceIsPending, 1, 0) == 0)
            {
#endif
                RemoveVoidTables();

                var thread = new System.Threading.Thread(
                    new ThreadStart(
                        delegate
                        {
                            try
                            {
                                var pos = _TableList;

                                while (pos != null)
                                {
                                    var target = (IMaintainable)pos.Target;

                                    if (target != null)
                                        target.DoMaintenance();

                                    pos = pos.Next;
                                }
                            }
                            finally
                            {
                                _TableMaintenanceIsPending = 0;
                            }
                        }
                    )
                );

#if !SILVERLIGHT
                thread.Priority = ThreadPriority.Highest;
#endif
                thread.Start();

            }
        }

        #endregion

        #region maintaining Tables list

        sealed class ListNode
        {
            public ListNode(object target)
            { _Reference = new WeakReference(target); }

            WeakReference _Reference;

            public object Target { get { return _Reference.Target; } }

            public ListNode Next;
        }

        /// <summary>
        /// a list of all WeakHashtables
        /// </summary>
        static ListNode _TableList;

        /// <summary>
        /// this is to be called from the constructor or initializer of a ConcurrentWeakHashtable instance
        /// </summary>
        internal static void Register(IMaintainable table)
        {
            var node = new ListNode(table);

            //Next may not be assigned correct value yet when garbage sweep starts
            //but this is not a very big deal.
            node.Next = _TableList;
            node.Next = Interlocked.Exchange(ref _TableList, node);
        }

        #endregion
    }
}

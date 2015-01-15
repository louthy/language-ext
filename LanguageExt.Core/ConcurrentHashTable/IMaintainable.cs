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
    /// <summary>
    /// An interface for objects that need regular maintenance executed on a background worker thread.
    /// </summary>
    public interface IMaintainable
    {
        /// <summary>
        /// Method called by a background worker thread to do maintenance deeded by the object.
        /// </summary>
        void DoMaintenance();
    }
}

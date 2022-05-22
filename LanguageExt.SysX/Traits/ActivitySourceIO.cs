using System;
using System.Diagnostics;
using LanguageExt.Attributes;

namespace LanguageExt.SysX.Traits
{
    public interface ActivitySourceIO
    {
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name and activity kind.
        /// </summary>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <returns>The created activity object, if it had active listeners, or `None` if it has no event
        /// listeners.</returns>
        Activity? StartActivity(string name, ActivityKind kind);

        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentContext">The parent `ActivityContext` object to initialize the created activity object
        /// with</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <param name="links">The optional `ActivityLink` list to initialise the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The created activity object, if it had active listeners, or null if it has no event listeners.</returns>
        Activity? StartActivity(
            string name, 
            ActivityKind kind, 
            ActivityContext parentContext,
            HashMap<string,object> tags = default, 
            Seq<ActivityLink> links = default, 
            DateTimeOffset startTime = default);

        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentId">The parent Id to initialize the created activity object with.</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <param name="links">The optional `ActivityLink` list to initialise the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The created activity object, if it had active listeners, or null if it has no event listeners.</returns>
        Activity? StartActivity(
            string name, 
            ActivityKind kind, 
            string parentId,
            HashMap<string,object> tags = default, 
            Seq<ActivityLink> links = default, 
            DateTimeOffset startTime = default);        
    }
        
    /// <summary>
    /// Type-class giving a struct the trait of supporting ActivitySource IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasActivitySource<RT>
        where RT : struct, HasActivitySource<RT>
    {
        /// <summary>
        /// Access the activity-source synchronous effect environment
        /// </summary>
        /// <returns>Activity-source synchronous effect environment</returns>
        Eff<RT, ActivitySourceIO> ActivitySourceEff { get; }
        
        /// <summary>
        /// Set the current activity and update the ParentId automatically
        /// </summary>
        /// <param name="activity">Activity to set</param>
        /// <returns>Updated runtime</returns>
        RT SetActivity(Activity? activity);

        /// <summary>
        /// Get the current activity 
        /// </summary>
        Activity? CurrentActivity { get; }
    }
}

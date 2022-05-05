using System;
using System.Diagnostics;
using LanguageExt.Effects.Traits;
using LanguageExt.SysX.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.SysX.Diag
{
    /// <summary>
    /// An `Activity` has an operation name, an ID, a start time and duration, tags, and baggage.
    /// 
    /// Activities should be created by calling the `span` functions, configured as necessary.  Each `span` function
    /// takes an `Eff` or `Aff` operation to run (which is the activity).  The runtime system will maintain the parent-
    /// child relationships for the activities, and maintains the 'current' activity.
    /// </summary>
    /// <typeparam name="RT"></typeparam>
    public static class Activity<RT>
        where RT : struct, HasActivitySource<RT>, HasCancel<RT>
    {
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <returns>The result of the `operation`</returns>
        public static Eff<RT, A> span<A>(string name, Eff<RT, A> operation) =>
            span(name, ActivityKind.Internal, default, default, DateTimeOffset.Now, operation);
        
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <returns>The result of the `operation`</returns>
        public static Aff<RT, A> span<A>(string name, Aff<RT, A> operation) =>
            span(name, ActivityKind.Internal, default, default, DateTimeOffset.Now, operation);
            
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <returns>The result of the `operation`</returns>
        public static Eff<RT, A> span<A>(string name, ActivityKind kind, Eff<RT, A> operation) =>
            span(name, kind, default, default, DateTimeOffset.Now, operation);
        
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <returns>The result of the `operation`</returns>
        public static Aff<RT, A> span<A>(string name, ActivityKind kind, Aff<RT, A> operation) =>
            span(name, kind, default, default, DateTimeOffset.Now, operation);
            
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <returns>The result of the `operation`</returns>
        public static Eff<RT, A> span<A>(string name, ActivityKind kind, HashMap<string, object> tags, Eff<RT, A> operation) =>
            span(name, kind, tags, default, DateTimeOffset.Now, operation);
        
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <returns>The result of the `operation`</returns>
        public static Aff<RT, A> span<A>(string name, ActivityKind kind, HashMap<string, object> tags, Aff<RT, A> operation) =>
            span(name, kind, tags, default, DateTimeOffset.Now, operation);
                    
        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentContext">The parent `ActivityContext` object to initialize the created activity object
        /// with</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <param name="links">The optional `ActivityLink` list to initialise the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The result of the `operation`</returns>
        public static Eff<RT, A> span<A>(
            string name, 
            ActivityKind kind, 
            ActivityContext parentContext,
            HashMap<string, object> tags, 
            Seq<ActivityLink> links, 
            DateTimeOffset startTime,
            Eff<RT, A> operation) =>
            use(default(RT).ActivitySourceEff.Map(rt => rt.StartActivity(
                    name, 
                    kind,
                    parentContext,
                    tags,
                    links,
                    startTime)),
                act => localEff<RT, RT, A>(rt => rt.SetActivity(act), operation));

        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="parentContext">The parent `ActivityContext` object to initialize the created activity object
        /// with</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <param name="links">The optional `ActivityLink` list to initialise the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The result of the `operation`</returns>
        public static Aff<RT, A> span<A>(
            string name, 
            ActivityKind kind, 
            ActivityContext parentContext,
            HashMap<string,object> tags, 
            Seq<ActivityLink> links, 
            DateTimeOffset startTime,
            Aff<RT, A> operation) =>
            use(default(RT).ActivitySourceEff.Map(rt => rt.StartActivity(
                    name, 
                    kind,
                    parentContext,
                    tags,
                    links,
                    startTime)),
                act => localAff<RT, RT, A>(rt => rt.SetActivity(act), operation));

        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <param name="links">The optional `ActivityLink` list to initialise the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The result of the `operation`</returns>
        static Eff<RT, A> span<A>(
            string name,
            ActivityKind kind,
            HashMap<string, object> tags,
            Seq<ActivityLink> links,
            DateTimeOffset startTime,
            Eff<RT, A> operation) =>
            from e in runtime<RT>()
            from r in use(e.ActivitySourceEff.Map(
                    rt => e.CurrentActivity == null
                        ? rt.StartActivity(name, kind)
                        : rt.StartActivity(
                            name,
                            kind,
                            e.CurrentActivity.Context,
                            tags,
                            links,
                            startTime)),
                act => localEff<RT, RT, A>(rt => rt.SetActivity(act), operation))
            select r;

        /// <summary>
        /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
        /// activity context, tags, optional activity link and optional start time.
        /// </summary>
        /// <param name="operation">The operation to whose activity will be traced</param>
        /// <param name="name">The operation name of the activity.</param>
        /// <param name="kind">The activity kind.</param>
        /// <param name="tags">The optional tags list to initialise the created activity object with.</param>
        /// <param name="links">The optional `ActivityLink` list to initialise the created activity object with.</param>
        /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
        /// <returns>The result of the `operation`</returns>
        static Aff<RT, A> span<A>(
            string name,
            ActivityKind kind,
            HashMap<string, object> tags,
            Seq<ActivityLink> links,
            DateTimeOffset startTime,
            Aff<RT, A> operation) =>
            from e in runtime<RT>()
            from r in use(e.ActivitySourceEff.Map(
                    rt => e.CurrentActivity == null
                            ? rt.StartActivity(name, kind)
                            : rt.StartActivity(
                                name,
                                kind,
                                e.CurrentActivity.Context,
                                tags,
                                links,
                                startTime)),
                act => localAff<RT, RT, A>(rt => rt.SetActivity(act), operation))
            select r;

        /// <summary>
        /// Set the state trace string
        /// </summary>
        /// <param name="traceStateString">Trace state string</param>
        /// <returns>Unit effect</returns>
        public static Eff<RT, Unit> setTraceState(string traceStateString) =>
            Eff<RT, Unit>(rt =>
            {
                if (rt.CurrentActivity is not null)
                {
                    rt.CurrentActivity.TraceStateString = traceStateString;
                }
                return unit;
            });

        /// <summary>
        /// Read the trace-state string of the current activity
        /// </summary>
        public static Eff<RT, Option<string>> traceState =>
            Eff<RT, Option<string>>(rt => rt.CurrentActivity?.TraceStateString);        

        /// <summary>
        /// Read the trace ID of the current activity
        /// </summary>
        public static Eff<RT, Option<ActivityTraceId>> traceId =>
            Eff<RT, Option<ActivityTraceId>>(rt => Optional(rt.CurrentActivity?.TraceId));        

        /// <summary>
        /// Add baggage to the current activity
        /// </summary>
        /// <param name="key">Baggage key</param>
        /// <param name="value">Baggage value</param>
        /// <returns>Unit effect</returns>
        public static Eff<RT, Unit> addBaggage(string key, string? value) =>
            Eff<RT, Unit>(rt =>
            {
                if (rt.CurrentActivity is not null)
                {
                    rt.CurrentActivity.AddBaggage(key, value);
                }
                return unit;
            });

        /// <summary>
        /// Read the baggage of the current activity
        /// </summary>
        public static Eff<RT, HashMap<string, string?>> baggage =>
            Eff<RT, HashMap<string, string?>>(rt =>
                rt.CurrentActivity is not null
                    ? rt.CurrentActivity.Baggage.ToHashMap()
                    :  HashMap<string, string?>());        

        /// <summary>
        /// Add tag to the current activity
        /// </summary>
        /// <param name="name">Tag name</param>
        /// <param name="value">Tag value</param>
        /// <returns>Unit effect</returns>
        public static Eff<RT, Unit> addTag(string name, string? value) =>
            Eff<RT, Unit>(rt =>
            {
                if (rt.CurrentActivity is not null)
                {
                    rt.CurrentActivity.AddTag(name, value);
                }
                return unit;
            });

        /// <summary>
        /// Add tag to the current activity
        /// </summary>
        /// <param name="name">Tag name</param>
        /// <param name="value">Tag value</param>
        public static Eff<RT, Unit> addTag(string name, object? value) =>
            Eff<RT, Unit>(rt =>
            {
                if (rt.CurrentActivity is not null)
                {
                    rt.CurrentActivity.AddTag(name, value);
                }
                return unit;
            });

        /// <summary>
        /// Read the tags of the current activity
        /// </summary>
        public static Eff<RT, HashMap<string, string?>> tags =>
            Eff<RT, HashMap<string, string?>>(rt =>
                rt.CurrentActivity is not null
                    ? rt.CurrentActivity.Tags.ToHashMap()
                    :  HashMap<string, string?>());        

        /// <summary>
        /// Read the tags of the current activity
        /// </summary>
        public static Eff<RT, HashMap<string, object?>> tagObjects =>
            Eff<RT, HashMap<string, object?>>(rt =>
                rt.CurrentActivity is not null
                    ? rt.CurrentActivity.TagObjects.ToHashMap()
                    :  HashMap<string, object?>());        

        /// <summary>
        /// Read the context of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<ActivityContext>> context =>
            Eff<RT, Option<ActivityContext>>(rt => Optional(rt.CurrentActivity?.Context));

        /// <summary>
        /// Read the duration of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<TimeSpan>> duration =>
            Eff<RT, Option<TimeSpan>>(rt => Optional(rt.CurrentActivity?.Duration));        

        /// <summary>
        /// Read the events of the current activity
        /// </summary>
        public static Eff<RT, Seq<ActivityEvent>> events =>
            Eff<RT, Seq<ActivityEvent>>(rt => 
                rt.CurrentActivity is not null
                    ? rt.CurrentActivity.Events.ToSeq()
                    :  Seq<ActivityEvent>());

        /// <summary>
        /// Read the ID of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<string>> id =>
            Eff<RT, Option<string>>(rt => Optional(rt.CurrentActivity?.Id));

        /// <summary>
        /// Read the kind of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<ActivityKind>> kind =>
            Eff<RT, Option<ActivityKind>>(rt => Optional(rt.CurrentActivity?.Kind));

        /// <summary>
        /// Read the links of the current activity
        /// </summary>
        public static Eff<RT, Seq<ActivityLink>> links =>
            Eff<RT, Seq<ActivityLink>>(rt => 
                rt.CurrentActivity is not null
                    ? rt.CurrentActivity.Links.ToSeq()
                    :  Seq<ActivityLink>());

        /// <summary>
        /// Read the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<Activity>> current =>
            Eff<RT, Option<Activity>>(rt => Optional(rt.CurrentActivity));

        /// <summary>
        /// Read the parent ID of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<string>> parentId =>
            Eff<RT, Option<string>>(rt => Optional(rt.CurrentActivity?.ParentId));

        /// <summary>
        /// Read the parent span ID of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<ActivitySpanId>> parentSpanId =>
            Eff<RT, Option<ActivitySpanId>>(rt => Optional(rt.CurrentActivity?.ParentSpanId));

        /// <summary>
        /// Read the recorded flag of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<bool>> recorded =>
            Eff<RT, Option<bool>>(rt => Optional(rt.CurrentActivity?.Recorded));

        /// <summary>
        /// Read the display-name of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<string>> displayName =>
            Eff<RT, Option<string>>(rt => Optional(rt.CurrentActivity?.DisplayName));

        /// <summary>
        /// Read the operation-name of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<string>> operationName =>
            Eff<RT, Option<string>>(rt => Optional(rt.CurrentActivity?.OperationName));

        /// <summary>
        /// Read the root ID of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<string>> rootId =>
            Eff<RT, Option<string>>(rt => Optional(rt.CurrentActivity?.RootId));

        /// <summary>
        /// Read the span ID of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<ActivitySpanId>> spanId =>
            Eff<RT, Option<ActivitySpanId>>(rt => Optional(rt.CurrentActivity?.SpanId));

        /// <summary>
        /// Read the start-time of the current activity
        /// </summary>
        /// <remarks>None if there is no current activity</remarks>
        public static Eff<RT, Option<DateTime>> startTimeUTC =>
            Eff<RT, Option<DateTime>>(rt => Optional(rt.CurrentActivity?.StartTimeUtc));
    }
}

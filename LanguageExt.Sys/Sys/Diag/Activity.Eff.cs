using System;
using System.Diagnostics;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Diag;

/// <summary>
/// An `Activity` has an operation name, an ID, a start time and duration, tags, and baggage.
/// 
/// Activities should be created by calling the `span` functions, configured as necessary.  Each `span` function
/// takes an `Eff` or `Aff` operation to run (which is the activity).  The runtime system will maintain the parent/
/// child relationships for the activities, and maintains the 'current' activity.
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
public class Activity<RT>
    where RT :
        Has<Eff<RT>, ActivitySourceIO>,
        Local<Eff<RT>, ActivityEnv>
{
    static Eff<RT, ActivityEnv> env =>
        Has<Eff<RT>, RT, ActivityEnv>.ask.As();

    static Eff<RT, Activity?> currentActivity =>
        env.Map(e => e.Activity);

    public static Eff<RT, Activity> startActivity(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        ActivityContext? parentContext = default) =>
        Activity<Eff<RT>, RT>.startActivity(
            name, 
            activityKind, 
            activityTags, 
            activityLinks, 
            startTime, 
            parentContext).As();

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static Eff<RT, A> span<A>(string name, K<Eff<RT>, A> operation) =>
        Activity<Eff<RT>, RT>.span(name, operation).As();

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static Eff<RT, A> span<A>(
        string name,
        ActivityKind activityKind,
        K<Eff<RT>, A> operation) =>
        Activity<Eff<RT>, RT>.span(name, activityKind, operation).As();

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="activityTags">The optional tags list to initialise the created activity object with.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static Eff<RT, A> span<A>(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        K<Eff<RT>, A> operation) =>
        Activity<Eff<RT>, RT>.span(name, activityKind, activityTags, operation).As();

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="activityTags">The optional tags list to initialise the created activity object with.</param>
    /// <param name="activityLinks">The optional `ActivityLink` list to initialise the created activity object with.</param>
    /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static Eff<RT, TA> span<TA>(
        string name,
        ActivityKind activityKind,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        K<Eff<RT>, TA> operation) =>
        Activity<Eff<RT>, RT>.span(name, activityKind, activityTags, activityLinks, startTime, operation).As();

    /// <summary>
    /// Creates a new activity if there are active listeners for it, using the specified name, activity kind, parent
    /// activity context, tags, optional activity link and optional start time.
    /// </summary>
    /// <param name="name">The operation name of the activity.</param>
    /// <param name="activityKind">The activity kind.</param>
    /// <param name="parentContext">The parent `ActivityContext` object to initialize the created activity object
    /// with</param>
    /// <param name="activityTags">The optional tags list to initialise the created activity object with.</param>
    /// <param name="activityLinks">The optional `ActivityLink` list to initialise the created activity object with.</param>
    /// <param name="startTime">The optional start timestamp to set on the created activity object.</param>
    /// <param name="operation">The operation to whose activity will be traced</param>
    /// <returns>The result of the `operation`</returns>
    public static Eff<RT, A> span<A>(
        string name,
        ActivityKind activityKind,
        ActivityContext parentContext,
        HashMap<string, object> activityTags,
        Seq<ActivityLink> activityLinks,
        DateTimeOffset startTime,
        K<Eff<RT>, A> operation) =>
        Activity<Eff<RT>, RT>.span(
            name, 
            activityKind, 
            parentContext, 
            activityTags, 
            activityLinks, 
            startTime, 
            operation).As();

    /// <summary>
    /// Set the state trace string
    /// </summary>
    /// <param name="traceStateString">Trace state string</param>
    /// <returns>Unit effect</returns>
    public static Eff<RT, Unit> setTraceState(string traceStateString) =>
        Activity<Eff<RT>, RT>.setTraceState(traceStateString).As();

    /// <summary>
    /// Read the trace-state string of the current activity
    /// </summary>
    public static Eff<RT, Option<string>> traceState =>
        Activity<Eff<RT>, RT>.traceState.As();

    /// <summary>
    /// Read the trace ID of the current activity
    /// </summary>
    public static Eff<RT, Option<ActivityTraceId>> traceId =>
        Activity<Eff<RT>, RT>.traceId.As();

    /// <summary>
    /// Add baggage to the current activity
    /// </summary>
    /// <param name="key">Baggage key</param>
    /// <param name="value">Baggage value</param>
    /// <returns>Unit effect</returns>
    public static Eff<RT, Unit> addBaggage(string key, string? value) =>
        Activity<Eff<RT>, RT>.addBaggage(key, value).As();

    /// <summary>
    /// Read the baggage of the current activity
    /// </summary>
    public static Eff<RT, HashMap<string, string?>> baggage =>
        Activity<Eff<RT>, RT>.baggage.As();

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    /// <returns>Unit effect</returns>
    public static Eff<RT, Unit> addTag(string name, string? value) =>
        Activity<Eff<RT>, RT>.addTag(name, value).As();

    /// <summary>
    /// Add tag to the current activity
    /// </summary>
    /// <param name="name">Tag name</param>
    /// <param name="value">Tag value</param>
    public static Eff<RT, Unit> addTag(string name, object? value) =>
        Activity<Eff<RT>, RT>.addTag(name, value).As();

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static Eff<RT, HashMap<string, string?>> tags =>
        Activity<Eff<RT>, RT>.tags.As();

    /// <summary>
    /// Read the tags of the current activity
    /// </summary>
    public static Eff<RT, HashMap<string, object?>> tagObjects =>
        Activity<Eff<RT>, RT>.tagObjects.As();

    /// <summary>
    /// Read the context of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivityContext>> context =>
        Activity<Eff<RT>, RT>.context.As();

    /// <summary>
    /// Read the duration of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<TimeSpan>> duration =>
        Activity<Eff<RT>, RT>.duration.As();

    /// <summary>
    /// Add an event to the current activity
    /// </summary>
    /// <param name="event">Event</param>
    public static Eff<RT, Unit> addEvent(ActivityEvent @event) =>
        Activity<Eff<RT>, RT>.addEvent(@event).As();

    /// <summary>
    /// Read the events of the current activity
    /// </summary>
    public static Eff<RT, Seq<ActivityEvent>> events =>
        Activity<Eff<RT>, RT>.events.As();

    /// <summary>
    /// Read the ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> id =>
        Activity<Eff<RT>, RT>.id.As();

    /// <summary>
    /// Read the kind of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivityKind>> kind =>
        Activity<Eff<RT>, RT>.kind.As();

    /// <summary>
    /// Read the links of the current activity
    /// </summary>
    public static Eff<RT, Seq<ActivityLink>> links =>
        Activity<Eff<RT>, RT>.links.As();

    /// <summary>
    /// Read the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<Activity>> current =>
        Activity<Eff<RT>, RT>.current.As();

    /// <summary>
    /// Read the parent ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> parentId =>
        Activity<Eff<RT>, RT>.parentId.As();

    /// <summary>
    /// Read the parent span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivitySpanId>> parentSpanId =>
        Activity<Eff<RT>, RT>.parentSpanId.As();

    /// <summary>
    /// Read the recorded flag of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<bool>> recorded =>
        Activity<Eff<RT>, RT>.recorded.As();

    /// <summary>
    /// Read the display-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> displayName =>
        Activity<Eff<RT>, RT>.displayName.As();

    /// <summary>
    /// Read the operation-name of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> operationName =>
        Activity<Eff<RT>, RT>.operationName.As();

    /// <summary>
    /// Read the root ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<string>> rootId =>
        Activity<Eff<RT>, RT>.rootId.As();

    /// <summary>
    /// Read the span ID of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<ActivitySpanId>> spanId =>
        Activity<Eff<RT>, RT>.spanId.As();

    /// <summary>
    /// Read the start-time of the current activity
    /// </summary>
    /// <remarks>None if there is no current activity</remarks>
    public static Eff<RT, Option<DateTime>> startTimeUTC =>
        Activity<Eff<RT>, RT>.startTimeUTC.As();
}

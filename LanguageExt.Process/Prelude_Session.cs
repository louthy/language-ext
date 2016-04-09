using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;
using System.Security.Cryptography;
using LanguageExt.Session;
using System.Reactive.Subjects;

namespace LanguageExt
{
    /// <summary>
    /// 
    ///     Process: Session functions
    /// 
    ///     These functions facilitate the use of sessions that live from
    ///     Process to Process.  Essentially if there's an active Session
    ///     ID then it will be packaged with each message that is sent via
    ///     tell or ask.  
    /// 
    /// </summary>
    public static partial class Process
    {
        internal static Subject<SessionId> sessionEnded = new Subject<SessionId>();
        internal static Subject<SessionId> sessionStarted = new Subject<SessionId>();
        public static readonly IObservable<SessionId> SessionEnded = sessionEnded;
        public static readonly IObservable<SessionId> SessionStarted = sessionStarted;

        /// <summary>
        /// Starts a new session in the Process system with the specified
        /// session ID
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="timeout">Session timeout</param>
        public static SessionId sessionStart(SessionId sid, Time timeout, SystemName system)
        {
            ActorContext.System(system).Sessions.Start(sid, (int)(timeout/1.Seconds()));
            ActorContext.SessionId = sid;
            return sid;
        }

        /// <summary>
        /// Starts a new session in the Process system.  This variant must be called from
        /// within a Process, use the variant where you specify the SystemName to use it
        /// from outside
        /// </summary>
        /// <param name="timeout">Session timeout</param>
        /// <returns>sid</returns>
        public static SessionId sessionStart(SessionId sid, Time timeout) =>
            InMessageLoop
                ? sessionStart(sid, timeout, ActorContext.Request.System.SystemName)
                : raiseUseInMsgLoopOnlyException<SessionId>(nameof(sessionStart));

        /// <summary>
        /// Starts a new session in the Process system.  This variant must be called from
        /// within a Process, use the variant where you specify the SystemName to use it
        /// from outside
        /// </summary>
        /// <param name="timeout">Session timeout</param>
        /// <returns>Session ID of the newly created session</returns>
        public static SessionId sessionStart(Time timeout) =>
            InMessageLoop
                ? sessionStart(SessionId.Generate(), timeout, ActorContext.Request.System.SystemName)
                : raiseUseInMsgLoopOnlyException<SessionId>(nameof(sessionStart));

        /// <summary>
        /// Ends a session in the Process system with the specified
        /// session ID
        /// </summary>
        /// <param name="sid">Session ID</param>
        public static Unit sessionStop() =>
            InMessageLoop
                ? ActorContext.SessionId.Iter(sid => ActorContext.Request.System.Sessions.Stop(sid))
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(sessionTouch));

        /// <summary>
        /// Touch a session
        /// Time-stamps the session so that its time-to-expiry is reset
        /// </summary>
        /// <param name="sid">Session ID</SessionId>
        public static Unit sessionTouch() =>
            InMessageLoop
                ? ActorContext.SessionId.Iter(sid => ActorContext.Request.System.Sessions.Touch(sid))
                : raiseUseInMsgLoopOnlyException<Unit>(nameof(sessionTouch));

        /// <summary>
        /// Gets the current session ID
        /// </summary>
        /// <remarks>Also touches the session so that its time-to-expiry 
        /// is reset</remarks>
        /// <returns>Optional session ID</returns>
        public static Option<SessionId> sessionId()
        {
            if (InMessageLoop)
            {
                var sid = ActorContext.SessionId;
                sid.IfSome(x => sessionTouch());
                return sid;
            }
            else
            {
                return raiseUseInMsgLoopOnlyException<Option<SessionId>>(nameof(sessionId));
            }
        }

        /// <summary>
        /// Set the meta-data to store with the session, this is typically
        /// user credentials when they've logged in.  But can be anything.  It is a 
        /// key/value store that is sync'd around the cluster.
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="key">Key</param>
        /// <param name="value">Data value </param>
        public static Unit sessionSetData(string key, object value)
        {
            if (InMessageLoop)
            {
                var session = from sid in ActorContext.SessionId
                              from ses in ActorContext.Request.System.Sessions.GetSession(sid)
                              select ses;

                if (session.IsNone)
                {
                    throw new Exception("Session not started");
                }

                var time = (from sess in session
                            from data in sess.Data.Find(key)
                            select data.Time)
                           .IfNone(0L);

                return ActorContext.Request.System.Sessions.SetData(time + 1, ActorContext.SessionId.IfNone(default(SessionId)), key, value);
            }
            else
            {
                return raiseUseInMsgLoopOnlyException<Unit>(nameof(sessionSetData));
            }
        }

        /// <summary>
        /// Clear the meta-data key stored with the session
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="key">Key</param>
        public static Unit sessionClearData(string key)
        {
            if (InMessageLoop)
            {
                var vect = (from sid in ActorContext.SessionId
                            from session in ActorContext.Request.System.Sessions.GetSession(sid)
                            from data in session.Data.Find(key)
                            select Tuple(sid, data.Time))
                           .IfNone(Tuple(default(SessionId), 0L));

                if (vect.Item2 == 0L)
                {
                    return unit;
                }

                return ActorContext.Request.System.Sessions.ClearData(vect.Item2, vect.Item1, key);
            }
            else
            {
                return raiseUseInMsgLoopOnlyException<Unit>(nameof(sessionClearData));
            }
        }

        /// <summary>
        /// Get the meta-data stored with the session.  
        /// <para>
        /// The session system allows concurrent updates from across the
        /// cluster or from within the app-domain (from multiple processes).
        /// To maintain the integrity of the data in any one session, the system 
        /// uses a version clock per-key.
        /// </para>
        /// <para>
        /// That means that if two Processes update the session from the
        /// same 'time' start point, then there will be a conflict and the session 
        /// will  contain both values stored against the key.  
        /// </para>
        /// <para>
        /// It is up to you  to decide on the best approach to resolving the conflict.  
        /// Calling Head() / HeadOrNone() on the result will get the value that was 
        /// written first, calling Last() will get the value that was written last.
        /// However, being first or last doesn't necessarily make a value 'right', in
        /// an asynchronous system the last value could be the newest or oldest.
        /// Both value commits had the same starting state, so if the consistency
        /// of the session data is important to you then you should implement
        /// a more robust strategy to deal with value conflicts, if integrity doesn't
        /// really matter, call HeadOrNone().
        /// </para>
        /// <para>
        /// The versioning system is closest to Lamport Clocks.  Eventually this 
        /// implementation will be replaced with a Dotted Version Vector system.
        /// </para>
        /// </summary>
        /// <param name="sid">Session ID</param>
        public static Lst<T> sessionGetData<T>(string key) =>
            InMessageLoop
                ? (from sessionId in ActorContext.SessionId
                   from session   in ActorContext.Request.System.Sessions.GetSession(sessionId)
                   from vector    in session.Data.Find(key)
                   select vector.Vector.Map(obj =>
                       obj is T
                           ? (T)obj
                           : default(T)))
                  .IfNone(List.empty<T>())
                  .Filter(notnull)
                :  raiseUseInMsgLoopOnlyException<Lst<T>>(nameof(sessionGetData));

        /// <summary>
        /// Returns True if there is a session ID available.  NOTE: That
        /// doesn't mean the session is still alive.
        /// </summary>
        /// <returns></returns>
        public static bool hasSession() =>
            ActorContext.SessionId.IsSome;

        /// <summary>
        /// Acquires a session for the duration of invocation of the 
        /// provided function. NOTE: This does not create a session, or
        /// check that a session exists.  
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="f">Function to invoke</param>
        /// <returns>Result of the function</returns>
        public static R withSession<R>(SessionId sid, Func<R> f) =>
            InMessageLoop
                ? ActorContext.Request.System.WithContext(
                    ActorContext.Request.Self,
                    ActorContext.Request.Self.Actor.Parent,
                    Process.Sender,
                    ActorContext.Request.CurrentRequest,
                    ActorContext.Request.CurrentMsg,
                    Some(sid),
                    f)
                : raiseUseInMsgLoopOnlyException<R>(nameof(withSession));

        /// <summary>
        /// Acquires a session ID for the duration of invocation of the 
        /// provided action.  NOTE: This does not create a session, or
        /// check that a session exists.  
        /// </summary>
        /// <param name="sid">Session ID</param>
        /// <param name="f">Action to invoke</param>
        public static Unit withSession(SessionId sid, Action f) =>
            withSession(sid, fun(f));
    }
}

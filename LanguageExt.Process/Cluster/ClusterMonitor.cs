using LanguageExt.UnitsOfMeasure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using static LanguageExt.Process;

namespace LanguageExt
{
    /// <summary>
    /// Process that monitors the state of the cluster
    /// </summary>
    class ClusterMonitor
    {
        public const string MembersKey = "sys-cluster-members";
        static readonly Time HeartbeatFreq = 1*seconds;
        static readonly Time OfflineCutoff = 3*seconds;

        public enum MsgTag
        {
            Heartbeat,
            ClusterMembersUpdated
        }

        public class Msg
        {
            public readonly MsgTag Tag;

            public Msg(MsgTag tag)
            {
                Tag = tag;
            }
        }

        public class State
        {
            public readonly Map<ProcessName, ClusterNode> Members;
            public readonly SystemName System;

            public static readonly State Empty = new State(Map.empty<ProcessName, ClusterNode>(), default(SystemName));

            public State(Map<ProcessName, ClusterNode> members, SystemName system)
            {
                Members = members.Filter(node => node != null);
                System = system;
            }

            public State SetMember(ProcessName nodeName, ClusterNode state) =>
                state == null
                    ? RemoveMember(nodeName)
                    : new State(Members.AddOrUpdate(nodeName, state), System);

            public State RemoveMember(ProcessName nodeName) =>
                new State(Members.Remove(nodeName), System);
        }

        /// <summary>
        /// Root Process setup
        /// </summary>
        public static State Setup(SystemName system)
        {
            return Heartbeat(State.Empty, ActorContext.System(system).Cluster);
        }

        /// <summary>
        /// Root Process inbox
        /// </summary>
        public static State Inbox(State state, Msg msg)
        {
            switch (msg.Tag)
            {
                case MsgTag.Heartbeat:
                    state = Heartbeat(state, ActorContext.System(state.System).Cluster);
                    tellSelf(new Msg(MsgTag.Heartbeat), HeartbeatFreq + (random(1000)*milliseconds));
                    return state;
            }
            return state;
        }

        /// <summary>
        /// If this node is part of a cluster then it updates a shared map of 
        /// node-names to states.  This also downloads the latest map so the
        /// cluster state is known locally.
        /// </summary>
        /// <param name="state">Current state</param>
        /// <returns>Latest state from the cluster, or a map with just one item 'root'
        /// in it that represents this node.</returns>
        static State Heartbeat(State state, Option<ICluster> cluster) =>
            cluster.Map(
                c =>
                {
                    try
                    {
                        var cutOff = DateTime.UtcNow.Add(0 * seconds - OfflineCutoff);

                        c.HashFieldAddOrUpdate(MembersKey, c.NodeName.Value, new ClusterNode(c.NodeName, DateTime.UtcNow, c.Role));
                        var newState = new State(c.GetHashFields<ProcessName, ClusterNode>(MembersKey, s => new ProcessName(s))
                                                  .Where(m => m.LastHeartbeat > cutOff), state.System);
                        var diffs = DiffState(state, newState);

                        diffs.Item1.Iter(offline => publish(state.Members[offline]));
                        diffs.Item2.Iter(online  => publish(newState.Members[online]));

                        return newState;
                    }
                    catch(Exception e)
                    {
                        logErr(e);
                        return HeartbeatLocal(state);
                    }
                })
            .IfNone(HeartbeatLocal(state));

        static Tuple<Set<ProcessName>, Set<ProcessName>> DiffState(State oldState, State newState)
        {
            var oldSet = Set.createRange(oldState.Members.Keys);
            var newSet = Set.createRange(newState.Members.Keys);
            return Tuple(oldSet - newSet, newSet - oldSet);
        }

        static State HeartbeatLocal(State state) =>
            state.SetMember("root", new ClusterNode("root", DateTime.UtcNow, "local"));

        static string GetNodeName(Option<ICluster> cluster) =>
            cluster.Map(c => c.NodeName.Value).IfNone("root");
    }
}

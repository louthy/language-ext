using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;
using LanguageExt.ClassInstances;

namespace TestBed
{
    public abstract class RobotState
    {
        public class Unknown : RobotState { public Unknown() { } }
        public class Up : RobotState { public Up() { } }
        public class Down : RobotState { public Down() { } }
    }

    public interface Env
    {
        MRobot<RobotState> WhereAreYou();
        MRobot<Unit> MoveRobotUp();
        MRobot<Unit> MoveRobotDown();
    }

    [LanguageExt.RWS(WriterMonoid: typeof(MUnit), Env: typeof(Env), State: typeof(RobotState))]
    public partial struct MRobot<A>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExt.Process
{
    public class ProcessMetaData
    {
        public readonly string MsgTypeName;
        public readonly string StateTypeName;

        public ProcessMetaData(string msgTypeName, string stateTypeName)
        {
            MsgTypeName = msgTypeName;
            StateTypeName = stateTypeName;
        }

        public Type GetMsgType() =>
            Type.GetType(MsgTypeName);

        public Type GetStateType() =>
            Type.GetType(StateTypeName);
    }
}

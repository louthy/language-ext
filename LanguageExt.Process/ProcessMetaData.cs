using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class ProcessMetaData
    {
        public readonly string[] MsgTypeNames;
        public readonly string StateTypeName;

        public ProcessMetaData(string[] msgTypeNames, string stateTypeName)
        {
            MsgTypeNames = msgTypeNames;
            StateTypeName = stateTypeName;
        }

        public Type GetStateType() =>
            Type.GetType(StateTypeName);
    }
}

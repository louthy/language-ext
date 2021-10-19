using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Runtime.Serialization;
using System;

namespace LanguageExt.DataTypes.Serialisation
{
    [Serializable]
    public class ValidationData<MonoidFail, FAIL, SUCCESS> : 
        Record<ValidationData<MonoidFail, FAIL, SUCCESS>>
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        public readonly Validation.StateType State;
        public readonly SUCCESS Success;
        public readonly FAIL Fail;

        public ValidationData(Validation.StateType state, SUCCESS success, FAIL fail)
        {
            State = state;
            Success = success;
            Fail = fail;
        }
    }
}

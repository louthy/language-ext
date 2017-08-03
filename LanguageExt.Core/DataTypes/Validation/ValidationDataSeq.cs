using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Runtime.Serialization;
using System;

namespace LanguageExt.DataTypes.Serialisation
{
    public class ValidationData<FAIL, SUCCESS> : 
        Record<ValidationData<FAIL, SUCCESS>>
    {
        public readonly Validation.StateType State;
        public readonly SUCCESS Success;
        public readonly Lst<FAIL> Fail;

        public ValidationData(Validation.StateType state, SUCCESS success, Lst<FAIL> fail)
        {
            State = state;
            Success = success;
            Fail = fail;
        }

        public ValidationData(SerializationInfo info, StreamingContext ctx)
            : base(info, ctx) { }
    }
}

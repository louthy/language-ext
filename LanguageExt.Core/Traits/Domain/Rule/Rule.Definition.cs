
using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public interface Rule<SELF> 
    where SELF : Rule<SELF>, new()
{
    public static virtual SELF Instance { get; } = new();
}

public interface Rule<SELF, A> : Rule<SELF>
    where SELF : Rule<SELF, A>, new()
{
    public static abstract bool Check(A value);

    public static virtual Fin<Unit> Validate(A value, Func<SELF, A, Error> Fail) =>
        SELF.Check(value) ? Prelude.unit : Fail(SELF.Instance, value);

}

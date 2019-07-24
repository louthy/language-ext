using System;
using AccountingDSL.Data;
using LanguageExt;

namespace AccountingDSL.DSL
{
    public class Transform<A>
    {
        public class Return : Transform<A>
        {
            public readonly A Value;
            public Return(A value) => Value = value;
        }

        public class Fail : Transform<A>
        {
            public readonly string Value;
            public Fail(string value) => Value = value;
        }

        public class AllRows : Transform<A>
        {
            public readonly Func<Seq<AccountingRow>, Transform<A>> Next;
            public AllRows(Func<Seq<AccountingRow>, Transform<A>> next)
            {
                Next = next;
            }
        }

        public class FilterRows : Transform<A>
        {
            public readonly string Value;
            public readonly Func<Seq<AccountingRow>, Transform<A>> Next;
            public FilterRows(string value, Func<Seq<AccountingRow>, Transform<A>> next)
            {
                Value = value;
                Next = next;
            }
        }

        public class Log : Transform<A>
        {
            public readonly string Value;
            public readonly Func<Unit, Transform<A>> Next;
            public Log(string value, Func<Unit, Transform<A>> next)
            {
                Value = value;
                Next = next;
            }
        }

        public class Invoke : Transform<A>
        {
            public readonly string Func;
            public readonly object[] Args;
            public readonly Func<object, Transform<A>> Next;
            public Invoke(string func, object[] args, Func<object, Transform<A>> next)
            {
                Func = func;
                Args = args;
                Next = next;
            }
        }

        public class SetValue : Transform<A>
        {
            public readonly string Name;
            public readonly object Value;
            public readonly Func<Unit, Transform<A>> Next;
            public SetValue(string name, object value, Func<Unit, Transform<A>> next)
            {
                Name = name;
                Value = value;
                Next = next;
            }
        }

        public class GetValue : Transform<A>
        {
            public readonly string Name;
            public readonly Func<object, Transform<A>> Next;
            public GetValue(string name, Func<object, Transform<A>> next)
            {
                Name = name;
                Next = next;
            }
        }

        public class Compute : Transform<A>
        {
            public readonly ComputeOperation Operation;
            public readonly Func<Unit, Transform<A>> Next;
            public readonly SourceType SourceType;
            public Compute(ComputeOperation operation, SourceType sourceType, Func<Unit, Transform<A>> next)
            {
                Operation = operation;
                SourceType = sourceType;
                Next = next;
            }
        }

        public class Print : Transform<A>
        {
            public readonly PrintOperation Operation;
            public readonly Seq<string> Messages;
            public readonly Func<Unit, Transform<A>> Next;
            public Print(PrintOperation operation, Seq<string> messages, Func<Unit, Transform<A>> next)
            {
                Operation = operation;
                Messages = messages;
                Next = next;
            }
        }
    }
}

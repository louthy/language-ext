using System;
using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExt.Config
{
    public class FieldSpec
    {
        public readonly string Name;
        public readonly Func<TypeDef> Type;

        public FieldSpec(string name, Func<TypeDef> type)
        {
            Name = name;
            Type = type;
        }
    }

    public class FuncSpec
    {
        public readonly string Name;
        public readonly Func<TypeDef> Type;
        public readonly Func<Map<string, NamedValueToken>, object> Body;
        public readonly FieldSpec[] Args;

        FuncSpec(string name, Func<TypeDef> type, Func<Map<string, NamedValueToken>, object> body, params FieldSpec[] args)
        {
            Name = name;
            Args = args;
            Type = type;
            Body = body;
        }

        public static FuncSpec Property(string name, Func<TypeDef> type) =>
            new FuncSpec(
                name,
                type,
                locals => locals["value"].Value.Value,
                new FieldSpec("value", type)
            );

        public static FuncSpec Property(string name, Func<TypeDef> returnType, Func<TypeDef> propertyType, Func<object, object> body) =>
            new FuncSpec(
                name,
                returnType,
                locals => body(locals["value"].Value.Value),
                new FieldSpec("value", propertyType)
            );

        public static FuncSpec Attrs(string name, Func<TypeDef> type, Func<Map<string, object>, object> body, params FieldSpec[] args) =>
            new FuncSpec(
                name,
                type,
                locals => body(locals.Map(v => v.Value.Value)),
                args
            );

        internal static FuncSpec Special(string name, Func<TypeDef> type) =>
            new FuncSpec(
                name,
                type,
                locals => locals
            );
    }
}

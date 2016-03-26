using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public enum ArgumentTypeTag
    {
        Int,
        Double,
        String,
        Time,
        ProcessId,
        ProcessName,
        ProcessFlags,
        Array,
        Process,
        Strategy,
        StrategyMatch,
        StrategyRedirect,
        Directive,
        Map
    }

    public class ArgumentType
    {
        public readonly ArgumentTypeTag Tag;
        public readonly ArgumentType GenericType;
        public readonly SettingSpec[] Spec;

        ArgumentType(ArgumentTypeTag tag, SettingSpec[] spec = null)
        {
            Tag = tag;
            Spec = spec;
        }

        ArgumentType(ArgumentTypeTag tag, ArgumentType genericType)
        {
            Tag = tag;
            GenericType = genericType;
        }

        public readonly static ArgumentType Int = 
            new ArgumentType(ArgumentTypeTag.Int);

        public readonly static ArgumentType Double =
            new ArgumentType(ArgumentTypeTag.Double);

        public readonly static ArgumentType String =
            new ArgumentType(ArgumentTypeTag.String);

        public readonly static ArgumentType Time =
            new ArgumentType(ArgumentTypeTag.Time);

        public readonly static ArgumentType ProcessId =
            new ArgumentType(ArgumentTypeTag.ProcessId);

        public readonly static ArgumentType ProcessName =
            new ArgumentType(ArgumentTypeTag.ProcessName);

        public readonly static ArgumentType ProcessFlags =
            new ArgumentType(ArgumentTypeTag.ProcessFlags);

        public static ArgumentType Process(params SettingSpec[] spec) =>
            new ArgumentType(ArgumentTypeTag.Process, spec);

        public static ArgumentType Strategy(params SettingSpec[] spec) =>
            new ArgumentType(ArgumentTypeTag.Strategy, spec);

        public static ArgumentType StrategyMatch =>
            new ArgumentType(ArgumentTypeTag.StrategyMatch);

        public static ArgumentType StrategyRedirect =>
            new ArgumentType(ArgumentTypeTag.StrategyRedirect);

        public static ArgumentType Array(ArgumentType genericType) =>
            new ArgumentType(ArgumentTypeTag.Array, genericType);

        public static ArgumentType Map(ArgumentType genericType) =>
            new ArgumentType(ArgumentTypeTag.Map, genericType);

        public static ArgumentType Directive =>
            new ArgumentType(ArgumentTypeTag.Directive);
    }

    public class ArgumentSpec
    {
        public readonly string Name;
        public readonly ArgumentType Type;

        ArgumentSpec(string name, ArgumentType type)
        {
            Name = name;
            Type = type;
        }

        public static ArgumentSpec Int(string name) =>
            new ArgumentSpec(name, ArgumentType.Int);

        public static ArgumentSpec Double(string name) =>
            new ArgumentSpec(name, ArgumentType.Double);

        public static ArgumentSpec String(string name) =>
            new ArgumentSpec(name, ArgumentType.String);

        public static ArgumentSpec Time(string name) =>
            new ArgumentSpec(name, ArgumentType.Time);

        public static ArgumentSpec ProcessId(string name) =>
            new ArgumentSpec(name, ArgumentType.ProcessId);

        public static ArgumentSpec ProcessName(string name) =>
            new ArgumentSpec(name, ArgumentType.ProcessName);

        public static ArgumentSpec ProcessFlags(string name) =>
            new ArgumentSpec(name, ArgumentType.ProcessFlags);

        public static ArgumentSpec Process(string name, params SettingSpec[] spec) =>
            new ArgumentSpec(name, ArgumentType.Process(spec));

        public static ArgumentSpec Strategy(string name, params SettingSpec[] spec) =>
            new ArgumentSpec(name, ArgumentType.Strategy(spec));

        public static ArgumentSpec StrategyMatch(string name) =>
            new ArgumentSpec(name, ArgumentType.StrategyMatch);

        public static ArgumentSpec StrategyRedirect(string name) =>
            new ArgumentSpec(name, ArgumentType.StrategyRedirect);

        public static ArgumentSpec Array(string name, ArgumentType genericType) =>
            new ArgumentSpec(name, ArgumentType.Array(genericType));

        public static ArgumentSpec Map(string name, ArgumentType genericType) =>
            new ArgumentSpec(name, ArgumentType.Map(genericType));

        public static ArgumentSpec Directive(string name) =>
            new ArgumentSpec(name, ArgumentType.Directive);

    }

    public class ArgumentsSpec
    {
        public readonly ArgumentSpec[] Args;
        public readonly Func<Map<string,SettingValue>, object> Build;

        public ArgumentsSpec(Func<Map<string, SettingValue>, object> build, params ArgumentSpec[] args)
        {
            Args = args;
            Build = build;
        }

        public ArgumentsSpec(params ArgumentSpec[] args)
            :
            this(null, args)
        {
        }

        public static ArgumentsSpec Variant(Func<Map<string, SettingValue>, object> build, params ArgumentSpec[] args) =>
            new ArgumentsSpec(build, args);

        public static ArgumentsSpec Variant(params ArgumentSpec[] args) =>
            new ArgumentsSpec(args);
    }

    public class SettingSpec
    {
        public readonly string Name;
        public readonly ArgumentsSpec[] Variants;

        public SettingSpec(string name, params ArgumentsSpec[] variants)
        {
            Name = name;
            Variants = variants;
        }

        public static SettingSpec Attr(string name, params ArgumentsSpec[] variants) =>
            new SettingSpec(name, variants);

        public static SettingSpec AttrNoArgs(string name) =>
            new SettingSpec(name, new[] { new ArgumentsSpec() });

        public static SettingSpec AttrNoArgs(string name, Func<string, SettingValue, object> f) =>
            new SettingSpec(name, new[] { new ArgumentsSpec() });

        public static SettingSpec Attr(string name, params ArgumentSpec[] args) =>
            new SettingSpec(name, new [] { new ArgumentsSpec(args) });

        public static SettingSpec Attr(string name, Func<Map<string, SettingValue>, object> f,  params ArgumentSpec[] args) =>
            new SettingSpec(name, new[] { new ArgumentsSpec(f, args) });

    }
}

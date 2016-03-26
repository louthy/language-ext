using System;
using LanguageExt.UnitsOfMeasure;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    class Attr
    {
        public readonly string Name;
        public Attr(string name)
        {
            Name = name;
        }
    }

    class NumericAttr : Attr
    {
        public readonly int Value;

        public NumericAttr(string name, int value)
            :
            base(name)
        {
            Value = value;
        }
    }

    class StringAttr : Attr
    {
        public readonly string Value;

        public StringAttr(string name, string value)
            :
            base(name)
        {
            Value = value;
        }
    }

    class MessageDirectiveAttr : Attr
    {
        public readonly MessageDirective Value;

        public MessageDirectiveAttr(string name, MessageDirective value)
            :
            base(name)
        {
            Value = value;
        }
    }

    class DirectiveAttr : Attr
    {
        public readonly Directive Value;

        public DirectiveAttr(string name, Directive value)
            :
            base(name)
        {
            Value = value;
        }
    }

    class TimeAttr : Attr
    {
        public readonly Time Value;

        public TimeAttr(string name, int value, string unit)
            :
            base(name)
        {
            switch (unit)
            {
                case "m":
                case "min":
                case "mins":
                case "minute":
                case "minutes":
                    Value = value.Minutes();
                    break;
                case "s":
                case "sec":
                case "secs":
                case "second":
                case "seconds":
                    Value = value.Seconds();
                    break;
                case "hr":
                case "hour":
                case "hours":
                    Value = value.Hours();
                    break;

                default:
                    throw new Exception("Invalid time unit-of-measure: " + unit);
            }
        }

        public static Option<Time> TryParse(int value, string unit)
        {
            switch (unit)
            {
                case "m":
                case "min":
                case "mins":
                case "minute":
                case "minutes":
                    return value.Minutes();

                case "s":
                case "sec":
                case "secs":
                case "second":
                case "seconds":
                    return value.Seconds();

                case "hr":
                case "hour":
                case "hours":
                    return value.Hours();

                default:
                    return None;
            }
        }
    }
}

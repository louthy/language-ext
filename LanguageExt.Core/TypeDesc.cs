using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public enum ValueTypeDesc
    {
        Other,
        Bool,
        Char,
        SByte,
        Short,
        Int,
        Long,
        Byte,
        UShort,
        UInt,
        ULong,
        Float,
        Double,
        Decimal,
        Guid,
        DateTime
    }

    internal class TypeDesc<T>
    {
        public readonly static TypeDesc Default = 
            new TypeDesc(typeof(T));
    }

    internal class TypeDesc
    {
        public readonly bool IsNumeric;
        public readonly bool IsString;
        public readonly Option<ValueTypeDesc> ValueType;
        public readonly bool IsAppendable;
        public readonly bool IsSubtractable;

        public TypeDesc(Type type)
        {
            IsAppendable = typeof(IAppendable).IsAssignableFrom(type);
            IsSubtractable = typeof(ISubtractable).IsAssignableFrom(type);
            IsString = type == typeof(string);

            if (type.IsValueType)
            {
                var valueType = type == typeof(bool) ? ValueTypeDesc.Bool
                              : type == typeof(char) ? ValueTypeDesc.Char
                              : type == typeof(sbyte) ? ValueTypeDesc.SByte
                              : type == typeof(short) ? ValueTypeDesc.Short
                              : type == typeof(int) ? ValueTypeDesc.Int
                              : type == typeof(long) ? ValueTypeDesc.Long
                              : type == typeof(byte) ? ValueTypeDesc.Byte
                              : type == typeof(ushort) ? ValueTypeDesc.UShort
                              : type == typeof(uint) ? ValueTypeDesc.UInt
                              : type == typeof(ulong) ? ValueTypeDesc.ULong
                              : type == typeof(float) ? ValueTypeDesc.Float
                              : type == typeof(double) ? ValueTypeDesc.Double
                              : type == typeof(decimal) ? ValueTypeDesc.Decimal
                              : type == typeof(Guid) ? ValueTypeDesc.Guid
                              : type == typeof(DateTime) ? ValueTypeDesc.DateTime
                              : ValueTypeDesc.Other;

                IsNumeric = valueType != ValueTypeDesc.Guid &&
                            valueType != ValueTypeDesc.Bool &&
                            valueType != ValueTypeDesc.Other &&
                            valueType != ValueTypeDesc.DateTime;

                if (valueType != ValueTypeDesc.Other)
                {
                    ValueType = valueType;
                }
            }
        }

        public static T Append<T>(T lhs, T rhs, TypeDesc desc)
        {
            if (desc.IsNumeric)
            {
                return (T)AppendNumeric(lhs, rhs, desc);
            }
            else if (desc.IsString)
            {
                return (T)AppendString(lhs, rhs);
            }
            else if (desc.IsAppendable)
            {
                return (lhs as IAppendable<T>).Append(rhs);
            }
            return lhs;
        }

        public static T Subtract<T>(T lhs, T rhs, TypeDesc desc)
        {
            if (desc.IsNumeric)
            {
                return (T)SubtractNumeric(lhs, rhs, desc);
            }
            else if (desc.IsSubtractable)
            {
                return (lhs as ISubtractable<T>).Subtract(rhs);
            }
            return lhs;
        }

        private static object AppendNumeric(object lhs, object rhs, TypeDesc desc)
        {
            return match(desc.ValueType,
                Some: vt =>
                {
                    switch (vt)
                    {
                        case ValueTypeDesc.Int: return (int)lhs + (int)rhs;
                        case ValueTypeDesc.Long: return (long)lhs + (long)rhs;
                        case ValueTypeDesc.Short: return (short)lhs + (short)rhs;
                        case ValueTypeDesc.SByte: return (sbyte)lhs + (sbyte)rhs;
                        case ValueTypeDesc.Char: return (char)lhs + (char)rhs;
                        case ValueTypeDesc.UInt: return (uint)lhs + (uint)rhs;
                        case ValueTypeDesc.ULong: return (ulong)lhs + (ulong)rhs;
                        case ValueTypeDesc.UShort: return (ushort)lhs + (ushort)rhs;
                        case ValueTypeDesc.Byte: return (byte)lhs + (byte)rhs;
                        case ValueTypeDesc.Float: return (float)lhs + (float)rhs;
                        case ValueTypeDesc.Double: return (double)lhs + (double)rhs;
                        case ValueTypeDesc.Decimal: return (decimal)lhs + (decimal)rhs;
                        default: return lhs;
                    }
                },
                None: () => lhs
            );
        }

        private static object SubtractNumeric(object lhs, object rhs, TypeDesc desc)
        {
            return match(desc.ValueType,
                Some: vt =>
                {
                    switch (vt)
                    {
                        case ValueTypeDesc.Int: return (int)lhs - (int)rhs;
                        case ValueTypeDesc.Long: return (long)lhs - (long)rhs;
                        case ValueTypeDesc.Short: return (short)lhs - (short)rhs;
                        case ValueTypeDesc.SByte: return (sbyte)lhs - (sbyte)rhs;
                        case ValueTypeDesc.Char: return (char)lhs - (char)rhs;
                        case ValueTypeDesc.UInt: return (uint)lhs - (uint)rhs;
                        case ValueTypeDesc.ULong: return (ulong)lhs - (ulong)rhs;
                        case ValueTypeDesc.UShort: return (ushort)lhs - (ushort)rhs;
                        case ValueTypeDesc.Byte: return (byte)lhs - (byte)rhs;
                        case ValueTypeDesc.Float: return (float)lhs - (float)rhs;
                        case ValueTypeDesc.Double: return (double)lhs - (double)rhs;
                        case ValueTypeDesc.Decimal: return (decimal)lhs - (decimal)rhs;
                        default: return lhs;
                    }
                },
                None: () => lhs
            );
        }

        public static object AppendString(object lhs, object rhs)
        {
            return (string)lhs + (string)rhs;
        }
    }
}

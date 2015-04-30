using System;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal delegate bool CanConvertDel(ITypeDescriptorContext ctx, Type typ);
    internal delegate object ConvertToDel(ITypeDescriptorContext ctx, CultureInfo cult, object val, Type typ);
    internal delegate object ConvertFromDel(ITypeDescriptorContext ctx, CultureInfo cult, object val);

    public class OptionalTypeConverter : TypeConverter
    {
        static readonly Func<Type, MethodInfo> methods = null;
        static readonly MethodInfo optional = null;

        readonly TypeConv conv;

        static OptionalTypeConverter()
        {
            optional = (from info in typeof(Prelude).GetMethods()
                        where info.Name == "Optional"
                        select info)
                       .First();

            methods = Prelude.memo((Type valueType) => optional.MakeGenericMethod(valueType));
        }

        public OptionalTypeConverter(Type type)
        {
            conv = new TypeConv(type, methods, true);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            conv.CanConvertFrom(context, sourceType, (ctx, st) => base.CanConvertFrom(ctx, st));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            conv.ConvertFrom(context, culture, value, (ctx, cul, val) => base.ConvertFrom(ctx, cul, val));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            conv.CanConvertTo(context, destinationType, (ctx, dt) => base.CanConvertTo(context, destinationType));

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            conv.ConvertTo(context, culture, value, destinationType, (ctx, cul, val, dt) => base.ConvertTo(ctx, cul, val, dt));
    }

    public class SomeTypeConverter : TypeConverter
    {
        static readonly Func<Type, MethodInfo> methods = null;
        static readonly MethodInfo someCreate = null;

        readonly TypeConv conv;

        static SomeTypeConverter()
        {
            someCreate = (from info in typeof(Some).GetMethods()
                          where info.Name == "Create"
                          select info)
                         .First();

            methods = Prelude.memo((Type valueType) => someCreate.MakeGenericMethod(valueType));
        }

        public SomeTypeConverter(Type type)
        {
            conv = new TypeConv(type, methods, false);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            conv.CanConvertFrom(context, sourceType, (ctx, st) => base.CanConvertFrom(ctx, st));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            conv.ConvertFrom(context, culture, value, (ctx, cul, val) => base.ConvertFrom(ctx, cul, val));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            conv.CanConvertTo(context, destinationType, (ctx, dt) => base.CanConvertTo(context, destinationType));

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            conv.ConvertTo(context, culture, value, destinationType, (ctx, cul, val, dt) => base.ConvertTo(ctx, cul, val, dt));
    }

    /// <summary>
    /// Helper class to factor the common aspects of SomeTypeConverter and OptionalTypeConvertor.  This is 
    /// based loosly on the BCL implementation of the Nullable<T> type-provider.  I've done what I can to 
    /// tame the messyness and to make it as functional as possible (primarly by making all methods 
    /// into expressions).
    /// </summary>
    internal class TypeConv
    {
        readonly Type type;
        readonly Type simpleType;
        readonly TypeConverter simpleTypeConverter;
        readonly bool emptyStringIsNone;
        readonly Func<Type, MethodInfo> methods;

        public TypeConv(Type type, Func<Type, MethodInfo> methods, bool emptyStringIsNone)
        {
            this.type = type;
            this.methods = methods;
            this.simpleType = type.GetGenericArguments().Single();
            this.simpleTypeConverter = TypeDescriptor.GetConverter(simpleType);
            this.emptyStringIsNone = emptyStringIsNone;
        }

        public bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType, CanConvertDel baseCanConvertFrom) =>
            sourceType == simpleType ? true
          : simpleTypeConverter == null ? baseCanConvertFrom(context, sourceType)
          : simpleTypeConverter.CanConvertFrom(context, sourceType);

        public object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value, ConvertFromDel baseConvertFrom) =>
            value == null ? None
          : value.GetType() == simpleType ? methods(simpleType)?.Invoke(null, new[] { value })
          : IfEmptyStringIsNone(value) ? None
          : SimpleConvertFrom(context, culture, value, baseConvertFrom);

        public bool CanConvertTo(ITypeDescriptorContext context, Type destinationType, CanConvertDel baseCanConvertTo) =>
            destinationType == simpleType ? true
          : destinationType == typeof(InstanceDescriptor) ? true
          : SimpleCanConvertTo(context, destinationType, baseCanConvertTo);

        public object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType, ConvertToDel baseConvertTo) =>
            value == null
                ? ConvertToValueNull(context, culture, destinationType, baseConvertTo)
                : MatchConvertTo(context, culture, value, destinationType, baseConvertTo);

        private InstanceDescriptor NewInstanceDesc(object value) =>
            new InstanceDescriptor(
                type.GetConstructor(new Type[] { simpleType }),
                new object[] { value },
                true
                );

        private bool IfEmptyStringIsNone(object value) =>
            emptyStringIsNone && value is string && String.IsNullOrEmpty(value as string);

        private object SimpleConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value, ConvertFromDel baseConvertFrom) =>
            simpleTypeConverter == null     
                ? baseConvertFrom(context, culture, value)
                : methods(simpleType)?.Invoke( 
                    null, 
                    new[] { simpleTypeConverter.ConvertFrom(context, culture, value) } 
                    );            

        private bool SimpleCanConvertTo(ITypeDescriptorContext context, Type destinationType, CanConvertDel baseCanConvertTo) =>
            simpleTypeConverter == null
                ? baseCanConvertTo(context,destinationType)
                : simpleTypeConverter.CanConvertTo(context, destinationType);

        private object ConvertToValueNull(ITypeDescriptorContext context, CultureInfo culture, Type destinationType, ConvertToDel baseConvertTo ) =>
            destinationType == typeof(string)
                ? String.Empty
                : baseConvertTo(context, culture, null, destinationType);

        private object SimpleConvertTo(object x, ITypeDescriptorContext context, CultureInfo culture, Type destinationType, ConvertToDel baseConvertTo) =>
            simpleTypeConverter == null
                ? baseConvertTo(context, culture, x, destinationType)
                : simpleTypeConverter.ConvertTo(context, culture, x, destinationType);

        private object IsSomeConvertTo(object x, ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType, ConvertToDel baseConvertTo) =>
            destinationType == simpleType                   ? x
          : destinationType == typeof(InstanceDescriptor)   ? NewInstanceDesc(value)
          : x == null                                       ? ConvertToValueNull(context, culture, destinationType, baseConvertTo)
          : SimpleConvertTo(x,context,culture,destinationType,baseConvertTo);

        private object MatchConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType, ConvertToDel baseConvertTo) =>
            (value as IOptionalValue)?.MatchUntyped(
                Some: x  => IsSomeConvertTo(x, context, culture, value, destinationType, baseConvertTo),
                None: () => ConvertToValueNull(context, culture, destinationType, baseConvertTo));
    }
}

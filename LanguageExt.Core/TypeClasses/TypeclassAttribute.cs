using System;

namespace LanguageExt.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class TypeclassAttribute : Attribute
    {
        public readonly string NameFormat;
        public TypeclassAttribute(string nameFormat) =>
            NameFormat = nameFormat;
    }
}

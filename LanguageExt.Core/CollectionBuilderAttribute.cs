namespace System.Runtime.CompilerServices
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class CollectionBuilderAttribute : Attribute
    {
        public CollectionBuilderAttribute(
            Type builderType,
            string methodName)
        {
            BuilderType = builderType;
            MethodName = methodName;
        }

        public Type BuilderType { get; }
        public string MethodName { get; }
    }
}

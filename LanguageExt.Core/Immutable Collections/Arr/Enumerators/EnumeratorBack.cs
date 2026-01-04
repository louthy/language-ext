namespace LanguageExt;

public readonly partial struct Arr<A>
{
    /// <summary>
    /// Reversed enumerator
    /// </summary>
    public struct EnumeratorBack
    {
        readonly A[] arr;
        int index;
        int start;

        internal EnumeratorBack(in Arr<A> arr)
        {
            this.arr = arr.Value;
            start = arr.start;
            index = arr.start + arr.length;
        }

        public readonly A Current => 
            arr[index];

        public bool MoveNext() => 
            --index >= start;
    }
}

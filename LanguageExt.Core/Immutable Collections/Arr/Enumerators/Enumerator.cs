namespace LanguageExt;

public readonly partial struct Arr<A>
{
    /// <summary>
    /// Forward enumerator
    /// </summary>
    public struct Enumerator
    {
        readonly A[] arr;
        int index;
        int end;

        internal Enumerator(in Arr<A> arr)
        {
            this.arr = arr.Value;
            index = arr.start - 1;
            end = arr.start   + arr.length;
        }

        public readonly A Current => arr[index];

        public bool MoveNext() => 
            ++index < end;
    }
}

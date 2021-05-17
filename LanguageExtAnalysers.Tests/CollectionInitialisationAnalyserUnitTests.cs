using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = LanguageExtAnalysers.Tests.CSharpAnalyzerVerifier<LanguageExtAnalysers.CollectionInitialisationAnalyser>;

namespace LanguageExtAnalysers.Tests
{
    [TestClass]
    public class CollectionInitialisationAnalyserUnitTest
    {
        [TestMethod]
        public async Task EmptyCode() => await VerifyCS.VerifyAnalyzerAsync("");

        [TestMethod]
        public async Task FailForCollectionInitialiserWithLstFromLanguageExt()
        {
            var test = @"
                using System.Collections.Generic;
                using LanguageExt;

                namespace ConsoleApplication1
                {
                    class Test
                    {
                        public void Test1()
                        {
                            var immutableList = new Lst<int> { 4, 5, 6 };
                        }
                    }
                }
            ";

            var expected = VerifyCS.Diagnostic(CollectionInitialisationAnalyser.DiagnosticId)
                .WithLocation(line: 11, column: 62)
                .WithMessage(CollectionInitialisationAnalyser.MessageFormat);

            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        /// <summary>
        /// If there are no items in the initialiser then it doesn't actually do any harm
        /// </summary>
        [TestMethod]
        public async Task AllowEmptyCollectionInitialiserWithLstFromLanguageExt()
        {
            var test = @"
                using System.Collections.Generic;
                using LanguageExt;

                namespace ConsoleApplication1
                {
                    class Test
                    {
                        public void Test1()
                        {
                            var immutableList = new Lst<int> { };
                        }
                    }
                }
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task AllowCollectionInitialiserWithListFromBCL()
        {
            var test = @"
                using System.Collections.Generic;

                namespace ConsoleApplication1
                {
                    class Test
                    {
                        public void Test1()
                        {
                            var list1 = new List<int> { 1, 2, 3 };
                        }
                    }
                }
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task AllowCustomImmutableTypeWithCollectionInitialiser()
        {
            var test = @"
                using System;
                using System.Collections;
                using System.Collections.Generic;
                using System.Linq;

                namespace ConsoleApplication1
                {
                    class Test
                    {
                        public void Test1()
                        {
                            var list1 = new MyImmutableList<int> { 1, 2, 3 };
                        }
                    }

                    public class MyImmutableList<T> : IEnumerable<T>
                    {
                        private readonly T[] _items;
                        private MyImmutableList(T[] items) => _items = items;
                        public MyImmutableList() : this(Array.Empty<T>()) { }

                        public MyImmutableList<T> Add(T item) => new MyImmutableList<T>(_items.Append(item).ToArray());

                        public IEnumerator<T> GetEnumerator()
                        {
                            foreach (var item in _items)
                                yield return item;
                        }

                        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                    }
                }
            ";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}

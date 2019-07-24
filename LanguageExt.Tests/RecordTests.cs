using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LanguageExt.Tests
{
    public class RecordTests
    {
        /// <summary>
        /// Cons type - singly linked list
        /// </summary>
        public class Cons<A> : Record<Cons<A>>
        {
            public readonly A Head;
            public readonly Cons<A> Tail;

            public Cons(A head, Cons<A> tail)
            {
                Head = head;
                Tail = tail;
            }
        }

        [Fact]
        public void ConsTests()
        {
            var listA = new Cons<int>(1, new Cons<int>(2, new Cons<int>(3, new Cons<int>(4, null))));
            var listB = new Cons<int>(1, new Cons<int>(2, new Cons<int>(3, new Cons<int>(4, null))));
            var listC = new Cons<int>(1, new Cons<int>(2, new Cons<int>(3, null)));

            Assert.True(listA == listB);
            Assert.True(listB != listC);
            Assert.True(listA != listC);
        }

        /// <summary>
        /// Binary tree
        /// </summary>
        public class Tree<A> : Record<Tree<A>>
        {
            public readonly A Value;
            public readonly Tree<A> Left;
            public readonly Tree<A> Right;

            public Tree(A value, Tree<A> left, Tree<A> right)
            {
                Value = value;
                Left = left;
                Right = right;
            }
        }

        [Fact]
        public void TreeTests()
        {
            var treeA = new Tree<int>(5, new Tree<int>(3, null, null), new Tree<int>(7, null, new Tree<int>(9, null, null)));
            var treeB = new Tree<int>(5, new Tree<int>(3, null, null), new Tree<int>(7, null, new Tree<int>(9, null, null)));
            var treeC = new Tree<int>(5, new Tree<int>(3, null, null), new Tree<int>(7, null, null));

            Assert.True(treeA == treeB);
            Assert.True(treeB != treeC);
            Assert.True(treeA != treeC);

            var str = treeA.ToString();
        }


        class Disorder : Record<Disorder>
        {
            public string Present = "Here";
            public string Absent = null;
        }

        [Fact]
        public void NullMemberEqualityTest()
        {
            var a = new Disorder();
            var b = new Disorder();

            Assert.True(a == b);
            Assert.True(a.GetHashCode() == b.GetHashCode());
            Assert.False(a != b);
            Assert.False(a.GetHashCode() != b.GetHashCode());
        }

        [Fact]
        public void NullMemberOrderingTest()
        {
            var a = new Disorder();
            var b = new Disorder();

            Assert.True(a.CompareTo(b) == 0);
            Assert.True(b.CompareTo(a) == 0);
        }
    }
}

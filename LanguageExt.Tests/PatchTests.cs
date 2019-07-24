using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Patch;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests
{
    public class PatchTests
    {
        [Fact]
        public void PatchAppendEmptyIsPatch()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "World", "Again");

            var patchA = diff<EqString, string>(docA, docB);
            var patchB = append(patchA, Patch<EqString, string>.Empty);

            Assert.True(patchA == patchB);
        }

        [Fact]
        public void EmptyPatchAppendPatchIsPatch()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "World", "Again");

            var patchA = diff<EqString, string>(docA, docB);
            var patchB = append(Patch<EqString, string>.Empty, patchA);

            Assert.True(patchA == patchB);
        }

        [Fact]
        public void PatchCommutes()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "World", "Again");
            var docC = List("World");
            var docD = List("World", "War");

            var patchAB = diff<EqString, string>(docA, docB);
            var patchBC = diff<EqString, string>(docB, docC);
            var patchCD = diff<EqString, string>(docC, docD);

            var patchA_BC = append(patchAB, append(patchBC, patchCD));
            var patchAB_C = append(append(patchAB, patchBC), patchCD);

            Assert.True(patchA_BC == patchAB_C);
        }

        [Fact]
        public void InsertAtEndDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "World", "Again");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Insert.New(2, "Again"));
        }

        [Fact]
        public void InsertAtBeginningDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("Again", "Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Insert.New(0, "Again"));
        }

        [Fact]
        public void InsertAtMidDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "Again", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Insert.New(1, "Again"));
        }

        [Fact]
        public void InsertMultiDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("It's", "Hello", "Again", "World", "Cheers");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 3);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Insert.New(0, "It's"));
            Assert.True(patch.Edits.Tail.Head == Edit<EqString, string>.Insert.New(1, "Again"));
            Assert.True(patch.Edits.Tail.Tail.Head == Edit<EqString, string>.Insert.New(2, "Cheers"));
        }


        [Fact]
        public void DeleteAtEndDiff()
        {
            var docA = List("Hello", "World", "Again");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Delete.New(2, "Again"));
        }

        [Fact]
        public void DeleteAtBeginningDiff()
        {
            var docA = List("Again", "Hello", "World");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Delete.New(0, "Again"));
        }

        [Fact]
        public void DeleteAtMidDiff()
        {
            var docA = List("Hello", "Again", "World");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Delete.New(1, "Again"));
        }

        [Fact]
        public void DeleteMultiDiff()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 3);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Delete.New(0, "It's"));
            Assert.True(patch.Edits.Tail.Head == Edit<EqString, string>.Delete.New(2, "Again"));
            Assert.True(patch.Edits.Tail.Tail.Head == Edit<EqString, string>.Delete.New(4, "Cheers"));
        }


        [Fact]
        public void ReplaceAtEndDiff()
        {
            var docA = List("Hello", "World", "Again");
            var docB = List("Hello", "World", "Once More");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Replace.New(2, "Again", "Once More"));
        }

        [Fact]
        public void ReplaceAtBeginningDiff()
        {
            var docA = List("Again", "Hello", "World");
            var docB = List("Once More", "Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Replace.New(0, "Again", "Once More"));
        }

        [Fact]
        public void ReplaceAtMidDiff()
        {
            var docA = List("Hello", "Again", "World");
            var docB = List("Hello", "Once More", "World");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 1);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Replace.New(1, "Again", "Once More"));
        }

        [Fact]
        public void ReplaceMultiDiff()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var patch = diff<EqString, string>(docA, docB);

            Assert.True(patch.Edits.Count == 3);
            Assert.True(patch.Edits.Head == Edit<EqString, string>.Replace.New(0, "It's", "Yes"));
            Assert.True(patch.Edits.Tail.Head == Edit<EqString, string>.Replace.New(2, "Again", "My"));
            Assert.True(patch.Edits.Tail.Tail.Head == Edit<EqString, string>.Replace.New(4, "Cheers", "Of Joy"));
        }

        [Fact]
        public void ApplyInsertAtEndDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "World", "Again");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyInsertAtBeginningDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("Again", "Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyInsertAtMidDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("Hello", "Again", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyInsertMultiDiff()
        {
            var docA = List("Hello", "World");
            var docB = List("It's", "Hello", "Again", "World", "Cheers");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }


        [Fact]
        public void ApplyDeleteAtEndDiff()
        {
            var docA = List("Hello", "World", "Again");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyDeleteAtBeginningDiff()
        {
            var docA = List("Again", "Hello", "World");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyDeleteAtMidDiff()
        {
            var docA = List("Hello", "Again", "World");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyDeleteMultiDiff()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }


        [Fact]
        public void ApplyReplaceAtEndDiff()
        {
            var docA = List("Hello", "World", "Again");
            var docB = List("Hello", "World", "Once More");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyReplaceAtBeginningDiff()
        {
            var docA = List("Again", "Hello", "World");
            var docB = List("Once More", "Hello", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyReplaceAtMidDiff()
        {
            var docA = List("Hello", "Again", "World");
            var docB = List("Hello", "Once More", "World");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void ApplyReplaceMultiDiff()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var patch = diff<EqString, string>(docA, docB);

            var docC = apply(patch, docA);

            Assert.True(docB == docC);
        }

        [Fact]
        public void PatchAppendInversePatchIsEmpty()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var mp = default(MPatch<EqString, string>);

            var patch = diff<EqString, string>(docA, docB);
            var inverseP = inverse(patch);
            var patch1 = mp.Append(patch, inverseP);
            var empty = mp.Empty();

            Assert.True(patch1 == empty);
        }

        [Fact]
        public void InversePatchAppendPatchIsEmpty()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var mp = default(MPatch<EqString, string>);

            var patch = diff<EqString, string>(docA, docB);
            var inverseP = inverse(patch);

            var patch1 = mp.Append(inverseP, patch);
            var empty = mp.Empty();

            Assert.True(patch1 == empty);
        }

        [Fact]
        public void InverseInversePatchIsPatch()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var patch = diff<EqString, string>(docA, docB);
            var inverseP = inverse(patch);
            var inverseInverse = inverse(inverseP);

            Assert.True(patch == inverseInverse);
        }

        [Fact]
        public void PatchAppend()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");
            var docC = List("Yes", "My", "Of Joy");

            var patchP = diff<EqString, string>(docA, docB);
            var patchQ = diff<EqString, string>(docB, docC);

            var mp = default(MPatch<EqString, string>);

            var patchPQ = mp.Append(patchP, patchQ);

            var docD = apply(patchPQ, docA);

            var docD1 = apply(patchP, docA);
            var docD2 = apply(patchQ, docD1);

            Assert.True(docC == docD);

            var edits = patchPQ.Edits.ToArr();

            Assert.True(edits[0] == Edit<EqString, string>.Replace.New(0, "It's", "Yes"));
            Assert.True(edits[1] == Edit<EqString, string>.Delete.New(1, "Hello"));
            Assert.True(edits[2] == Edit<EqString, string>.Replace.New(2, "Again", "My"));
            Assert.True(edits[3] == Edit<EqString, string>.Delete.New(3, "World"));
            Assert.True(edits[4] == Edit<EqString, string>.Replace.New(4, "Cheers", "Of Joy"));
        }

        [Fact]
        public void InverseOfPatchPAppendPatchQIsInversePatchQAppendInversePatchP()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");
            var docC = List("Yes", "Hello", "My", "World", "Of Joy", "And More");

            var patchP = diff<EqString, string>(docA, docB);
            var patchQ = diff<EqString, string>(docB, docC);

            var mp = default(MPatch<EqString, string>);

            var invPatchP = inverse(patchP);
            var invPatchQ = inverse(patchQ);

            Assert.True(inverse(invPatchP) == patchP);
            Assert.True(inverse(invPatchQ) == patchQ);

            var patchPQ = mp.Append(patchP, patchQ);
            var invPatchPQ = inverse(patchPQ);

            Assert.True(inverse(invPatchPQ) == patchPQ);

            var invPatchQinvPatchP = mp.Append(invPatchQ, invPatchP);

            Assert.True(inverse(invPatchPQ) == inverse(invPatchQinvPatchP));

            Assert.True(invPatchPQ == invPatchQinvPatchP);
        }

        [Fact]
        public void InverseEmptyIsEmpty()
        {
            var empty = Patch<EqString, string>.Empty;
            Assert.True(empty == inverse(empty));
        }

        [Fact]
        public void ComposableWithInverse()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var patch = diff<EqString, string>(docA, docB);

            var isComposable = composable(patch, inverse(patch));

            Assert.True(isComposable);
        }

        [Fact]
        public void InverseComposableWith()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");

            var patch = diff<EqString, string>(docA, docB);

            var isComposable = composable(inverse(patch), patch);

            Assert.True(isComposable);
        }

        [Fact]
        public void InverseApplicableToAnyAppliedPatchOfP()
        {
            var docA = List("It's", "Hello", "Again", "World", "Cheers");
            var docB = List("Yes", "Hello", "My", "World", "Of Joy");
            var docC = List("Yes", "Hello", "My", "World", "Of Joy", "And More");

            var patch = diff<EqString, string>(docA, docB);

            var invPatch = inverse(patch);

            var isApplicable = applicable(invPatch, apply(patch, docC));

            Assert.True(isApplicable);
        }

        [Fact]
        public void ApplyDiffResultsInTargetDocument()
        {
            var d = List("It's", "Hello", "Again", "World", "Cheers");
            var e = List("Yes", "Hello", "My", "World", "Of Joy");

            Assert.True(apply(diff<EqString, string>(d, e), d) == e);
        }

        [Fact]
        public void DiffTheSameDocumentProducesEmptyPatch()
        {
            var d = List("It's", "Hello", "Again", "World", "Cheers");

            Assert.True(diff<EqString, string>(d, d) == empty<EqString, string>());
        }

        [Fact]
        public void DiffsBetweenThreePatchesAreEqualToDiffsBetweenFirstAndLastPatch()
        {
            var a = List("It's", "Hello", "Again", "World", "Cheers");
            var b = List("Yes", "Hello", "My", "World", "Of Joy");
            var c = List("Yes", "Hello", "My", "World", "Of Joy", "And More");

            Assert.True(
                apply(append(diff<EqString, string>(a, b), diff<EqString, string>(b, c)), a) == apply(diff<EqString, string>(a, c), a)
                );
        }

        [Fact]
        public void DiffWithConflictTakeOurs()
        {
            var a = List("Hello", "World");
            var b = List("World", "Hello");
            var c = List("Worldy", "Hello");

            var ab = diff<EqString, string>(a, b);
            var ac = diff<EqString, string>(a, c);

            var (pa, pb) = transformWith(ours, ab, ac);

            var newa = apply(pa, apply(ab, a));
            var newb = apply(pb, apply(ac, a));

            Assert.True(newa == newb);
            Assert.True(newa == b);
            Assert.True(newb == b);
        }

        [Fact]
        public void DiffWithConflictTakeTheirs()
        {
            var a = List("Hello", "World");
            var b = List("World", "Hello");
            var c = List("Worldy", "Hello");

            var ab = diff<EqString, string>(a, b);
            var ac = diff<EqString, string>(a, c);

            var (pa, pb) = transformWith(theirs, ab, ac);

            var newa = apply(pa, apply(ab, a));
            var newb = apply(pb, apply(ac, a));

            Assert.True(newa == newb);
            Assert.True(newa == c);
            Assert.True(newb == c);
        }

    }
}

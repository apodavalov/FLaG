using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetOfEquivalenceTransitionTest
    {
        private IEnumerable<char> symbols = EnumerateHelper.Sequence('a', 'c');

        [Test]
        public void CctorTest_SymbolsNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new SetOfEquivalenceTransition(null, 2));
        }

        [Test]
        public void CctorTest_Ok()
        {
            int expectedIndex = 6;

            SetOfEquivalenceTransition actual = new SetOfEquivalenceTransition(symbols, expectedIndex);

            CollectionAssert.AreEquivalent(symbols, actual.Symbols);
            Assert.AreEqual(expectedIndex, actual.IndexOfCurrentSetOfEquivalence);
        }
    }
}

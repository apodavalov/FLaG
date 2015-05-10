﻿using FLaGLib.Data;
using FLaGLib.Extensions;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FLaGLib.Collections;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetOfEquivalenceTransitionTest
    {
        private SetOfEquivalence setOfEquivalence1 =
            new SetOfEquivalence(
                EnumerateHelper.Sequence(
                    new Label(new SingleLabel('P')),
                    null,
                    new Label(new SingleLabel('D'))
                )                
            );

        private SetOfEquivalence setOfEquivalence2 =
            new SetOfEquivalence(
                EnumerateHelper.Sequence(
                    new Label(new SingleLabel('S')),
                    null,
                    new Label(new SingleLabel('M'))
                )   
            );

        private IEnumerable<char> symbols = EnumerateHelper.Sequence('a', 'c');

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_NextSetOfEquivalenceNull_Fail()
        {
            new SetOfEquivalenceTransition(null, symbols, setOfEquivalence1, 4);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_CurrentSetOfEquivalenceNull_Fail()
        {
            new SetOfEquivalenceTransition(setOfEquivalence1, symbols, null, 5);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SymbolsNull_Fail()
        {
            new SetOfEquivalenceTransition(setOfEquivalence1, null, setOfEquivalence2, 2);
        }

        [Test]
        public void CctorTest_Ok()
        {
            int expectedIndex = 6;

            SetOfEquivalenceTransition actual = new SetOfEquivalenceTransition(setOfEquivalence1, symbols, setOfEquivalence2, expectedIndex);

            Assert.AreEqual(setOfEquivalence1, actual.NextSetOfEquivalence);
            Assert.AreEqual(setOfEquivalence2, actual.CurrentSetOfEquivalence);
            CollectionAssert.AreEquivalent(symbols, actual.Symbols);
            Assert.AreEqual(expectedIndex, actual.IndexOfCurrentSetOfEquivalence);
        }
    }
}

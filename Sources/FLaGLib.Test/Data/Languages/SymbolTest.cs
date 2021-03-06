﻿using FLaGLib.Data.Languages;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class SymbolTest
    {
        private Tuple<Symbol, Symbol, int>[] _Expectations = new Tuple<Symbol, Symbol, int>[]
        {
            new Tuple<Symbol, Symbol, int>(
                null,
                null,
                0
            ),

            new Tuple<Symbol, Symbol, int>(
                new Symbol('a'),
                null,
                1
            ),

            new Tuple<Symbol, Symbol, int>(
                null,
                new Symbol('a'),
                -1
            ),

            new Tuple<Symbol, Symbol, int>(
                new Symbol('a'),
                new Symbol('a'),
                0
            ),

            new Tuple<Symbol, Symbol, int>(
                new Symbol('a'),
                new Symbol('b'),
                -1
            ),

            new Tuple<Symbol, Symbol, int>(
                new Symbol('b'),
                new Symbol('a'),
                1
            ),
        };

        [Test]
        public void CctorTest_Ok()
        {
            Symbol symbol = new Symbol('k');
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void EqualTest()
        {
            ComparableEquatableHelper.TestEquals(_Expectations);
        }

        [Test]
        public void GetHashCodeTest()
        {
            ComparableEquatableHelper.TestGetHashCode(_Expectations);
        }

        [Test]
        public void ToStringTest()
        {
            Symbol symbol = new Symbol('k');
            Assert.AreEqual("k", symbol.ToString());
        }

        [Test]
        public void ToRegExpTest()
        {
            Symbol symbol = new Symbol('k');
            symbol.ToRegExp();
        }
    }
}

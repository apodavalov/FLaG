using NUnit.Framework;
using RegExs = FLaGLib.Data.RegExps;
using Langs = FLaGLib.Data.Languages;
using FLaGLib.Helpers;
using FLaGLib.Data.Languages;
using System;
using FLaGLib.Test.TestHelpers;

namespace FLaGLib.Test.Data.Languages
{
    [TestFixture]
    public class TreeTest
    {
        private Tuple<Tree, Tree, int>[] _Expectations = new Tuple<Tree, Tree, int>[]
        {
            new Tuple<Tree, Tree, int>(
                null,
                null,
                0
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Union(
                                    EnumerateHelper.Sequence<Entity>(
                                        new Symbol('a'),
                                        new Symbol('b')
                                    )
                                )),
                null,
                1
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Union(
                                    EnumerateHelper.Sequence<Entity>(
                                        new Symbol('a'),
                                        new Symbol('b')
                                    )
                                )),
                new Tree(new Union(
                                    EnumerateHelper.Sequence<Entity>(
                                        new Symbol('a'),
                                        new Symbol('b')
                                    )
                                )),
                0
            ),

            new Tuple<Tree, Tree, int>(
                new Tree(new Union(
                                    EnumerateHelper.Sequence<Entity>(
                                        new Symbol('a'),
                                        new Symbol('b')
                                    )
                                )),
                new Tree(new Union(
                                    EnumerateHelper.Sequence<Entity>(
                                        new Symbol('c'),
                                        new Symbol('d')
                                    )
                                )),
                -2
            ),
        };

        [Test]
        public void ToRegExpTest()
        {      
            Langs.Symbol langSymbol1 = new Langs.Symbol('a');
            Langs.Symbol langSymbol2 = new Langs.Symbol('b');
            Langs.Concat langConcat1 = new Langs.Concat(EnumerateHelper.Sequence(langSymbol1, langSymbol2));
            Langs.Quantity langQuantity = new Langs.Quantity(2);  
            Langs.Degree langSubdegree = new Langs.Degree(langConcat1, langQuantity);
            Langs.Variable langVariable1 = new Langs.Variable('k', Langs.Sign.MoreThanOrEqual, 0);
            Langs.Degree langDegree1 = new Langs.Degree(langSubdegree, langVariable1);
            Langs.Symbol langSymbol3 = new Langs.Symbol('c');
            Langs.Concat langConcat2 = new Langs.Concat(EnumerateHelper.Sequence(langSymbol2, langSymbol3));
            Langs.Variable langVariable2 = new Langs.Variable('l', Langs.Sign.MoreThanOrEqual, 1);           
            Langs.Degree langDegree2 = new Langs.Degree(langConcat2, langVariable2);
            Langs.Variable langVariable3 = new Langs.Variable('m', Langs.Sign.MoreThanOrEqual, 2);
            Langs.Degree langDegree3 = new Langs.Degree(langSymbol1, langVariable3);
            Langs.Union langUnion = new Langs.Union(EnumerateHelper.Sequence(langDegree1, langDegree2, langDegree3));
            Langs.Tree langTree1 = new Langs.Tree(langConcat1);
            Langs.Tree langTree2 = new Langs.Tree(langConcat2);
            Langs.Tree langTree3 = new Langs.Tree(langDegree3);
            Langs.TreeCollection langTreeCollection = new Langs.TreeCollection(
                EnumerateHelper.Sequence(langTree1, langTree2, langTree3), Langs.TreeOperator.Union);

            Langs.Tree langTree = new Langs.Tree(langUnion, langTreeCollection);

            RegExs.Symbol regExSymbol1 = new RegExs.Symbol('a');
            RegExs.Symbol regExSymbol2 = new RegExs.Symbol('b');
            RegExs.BinaryConcat regExConcat1 = new RegExs.BinaryConcat(regExSymbol1, regExSymbol2);
            RegExs.ConstIteration regExConstIteration1 = new RegExs.ConstIteration(regExConcat1, 2);
            RegExs.Iteration regExIteration1 = new RegExs.Iteration(regExConstIteration1, false);
            RegExs.Symbol regExSymbol3 = new RegExs.Symbol('c');
            RegExs.BinaryConcat regExConcat2 = new RegExs.BinaryConcat(regExSymbol2, regExSymbol3);
            RegExs.Iteration regExIteration2 = new RegExs.Iteration(regExConcat2, true);
            RegExs.ConstIteration regExConstIteration2 = new RegExs.ConstIteration(regExSymbol1, 1);
            RegExs.Iteration regExIteration3 = new RegExs.Iteration(regExSymbol1,true);
            RegExs.BinaryConcat regExConcat3 = new RegExs.BinaryConcat(regExConstIteration2, regExIteration3);
            RegExs.BinaryUnion regExUnion = new RegExs.BinaryUnion(new RegExs.BinaryUnion(regExIteration2, regExIteration1), regExConcat3);
            RegExs.Tree regExTree1 = new RegExs.Tree(regExConcat1);
            RegExs.Tree regExTree2 = new RegExs.Tree(regExConcat2);
            RegExs.Tree regExTree3 = new RegExs.Tree(regExConcat3);
            RegExs.TreeCollection regExTreeCollection = new RegExs.TreeCollection(
                EnumerateHelper.Sequence(regExTree1, regExTree2, regExTree3), RegExs.TreeOperator.Union);

            RegExs.Tree expected = new RegExs.Tree(regExUnion, regExTreeCollection);

            RegExs.Tree actual = langTree.ToRegExp();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CcrotTest_Ok()
        {
            Tree tree = new Tree(new Union(
                                    EnumerateHelper.Sequence<Entity>(
                                        new Symbol('a'),
                                        new Symbol('b')
                                    )
                                ));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CcrotTest_SetNull_Fail()
        {
            Tree tree = new Tree(null);
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
    }
}

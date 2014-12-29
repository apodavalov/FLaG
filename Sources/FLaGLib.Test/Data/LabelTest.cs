﻿using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using FLaGLib.Data;
using FLaGLib.Test.TestHelpers;
using FLaGLib.Helpers;

namespace FLaGLib.Test.Data
{
    [TestFixture]
    public class LabelTest
    {
        private Tuple<Label, Label, int>[] _Expectations = new Tuple<Label, Label, int>[]
        {
            new Tuple<Label, Label, int>(null, null, 0),
            new Tuple<Label, Label, int>(null, new Label(new SingleLabel('b')), -1),
            new Tuple<Label, Label, int>(null, 
                new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') } )), -1),
            new Tuple<Label, Label, int>(new Label(new SingleLabel('b')), 
                new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') } )), -1),
            new Tuple<Label, Label, int>(new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') } )), 
                new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') } )), 0),
            new Tuple<Label, Label, int>(new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('c'), new SingleLabel('d') } )), 
                new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') } )), 1),
            new Tuple<Label, Label, int>(new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('c'), new SingleLabel('d'), new SingleLabel('e') } )), 
                new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') } )), 1)
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_ComplexNull_Fail()
        {
            new Label((ISet<SingleLabel>)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_ComplexEmpty_Fail()
        {
            new Label(new SortedSet<SingleLabel>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_AtLeastOneSingleLabelIsNull_Fail()
        {
            new Label(
                new SortedSet<SingleLabel>(
                    EnumerateHelper.Sequence(
                        new SingleLabel('S'),
                        null,
                        new SingleLabel('D')
                    )
                )
            );
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_SimpleNull_Fail()
        {
            new Label((SingleLabel)null);
        }

        [Test]
        public void CCtorTest_Complex_Ok()
        {
            List<SingleLabel> expectedLabels = new SingleLabel[]
            {
               new SingleLabel('c'),
               new SingleLabel('b')
            }.ToList();

            Label label = new Label(new HashSet<SingleLabel>(expectedLabels));

            expectedLabels.Sort();

            CollectionAssert.AreEqual(expectedLabels, label.Sublabels);
            Assert.AreEqual(LabelType.Complex, label.LabelType);
        }

        [Test]
        public void CCtorTest_Simple_Ok()
        {
            SingleLabel singleLabel = new SingleLabel('b');
            IReadOnlyList<SingleLabel> expectedLabels = new SingleLabel[]
            {
               singleLabel
            }.ToList().AsReadOnly();

            Label label = new Label(singleLabel);

            CollectionAssert.AreEqual(expectedLabels, label.Sublabels);
            Assert.AreEqual(LabelType.Simple, label.LabelType);
        }

        [Test]
        public void EqualsTest()
        {
            ComparableEquatableHelper.TestEquals(_Expectations);
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }

        [Test]
        public void ToStringTest_Complex()
        {
            Label label = new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') }));

            Assert.AreEqual("[{b_null_null_null}{c_null_null_null}]",label.ToString());
        }

        [Test]
        public void ToStringTest_Simple()
        {
            string actual = new Label(new SingleLabel('b')).ToString();
            string expected = "{b_null_null_null}";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTest_Ok()
        {
            Label expectedLabel = new Label(new SingleLabel('b', subIndex: 4));
            Label actualLabel = new Label(new SingleLabel('b', subIndex: 3)).Next();

            Assert.AreEqual(expectedLabel, actualLabel);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextTest_NonSimple_Fail()
        {
            new Label(new HashSet<SingleLabel>(
                new SingleLabel[] { new SingleLabel('b', 0, 1, 2), new SingleLabel('c', 0, 1, 2) })).Next();
        }

        [Test]
        public void ConvertToComplexTest_Ok()
        {
            Label expectedLabel = new Label(new HashSet<SingleLabel>(new SingleLabel[] { new SingleLabel('b') }));
            Label actualLabel = new Label(new SingleLabel('b')).ConvertToComplex();
            
            Assert.AreEqual(expectedLabel, actualLabel);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertToComplexTest_NonSimple_Fail()
        {
            new Label(new HashSet<SingleLabel>(
                new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') })).ConvertToComplex();
        }

        [Test]
        public void ExtractSingleLabelTest_Ok()
        {
            SingleLabel actualSingleLabel = new Label(new SingleLabel('b')).ExtractSingleLabel();
            SingleLabel expectedSingleLabel = new SingleLabel('b');

            Assert.AreEqual(expectedSingleLabel, actualSingleLabel);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExtractSingleLabelTest_NonSimple_Fail()
        {
            new Label(new HashSet<SingleLabel>(
                new SingleLabel[] { new SingleLabel('b'), new SingleLabel('c') })).ExtractSingleLabel();
        }
    }
}
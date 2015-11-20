using FLaGLib.Data;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
                new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))), -1),
            new Tuple<Label, Label, int>(new Label(new SingleLabel('b')), 
                new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))), -1),
            new Tuple<Label, Label, int>(new Label(EnumerateHelper.Sequence( new SingleLabel('b'), new SingleLabel('c'))), 
                new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))), 0),
            new Tuple<Label, Label, int>(new Label(EnumerateHelper.Sequence(new SingleLabel('c'), new SingleLabel('d'))), 
                new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))), 1),
            new Tuple<Label, Label, int>(new Label(EnumerateHelper.Sequence(new SingleLabel('c'), new SingleLabel('d'), new SingleLabel('e'))), 
                new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))), 1)
        };

        [Test]
        public void CctorTest_ComplexNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new Label((IEnumerable<SingleLabel>)null));
        }

        [Test]
        public void CctorTest_ComplexEmpty_Fail()
        {
            Assert.Throws<ArgumentException>(() => new Label(Enumerable.Empty<SingleLabel>()));
        }

        [Test]
        public void CctorTest_AnySingleLabelNull_Fail()
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new Label(
                        EnumerateHelper.Sequence(
                            new SingleLabel('S'),
                            null,
                            new SingleLabel('D')
                        )
                    )
            );
        }

        [Test]
        public void CctorTest_SimpleNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new Label((SingleLabel)null));
        }

        [Test]
        public void CCtorTest_Complex_Ok()
        {
            List<SingleLabel> expectedLabels = EnumerateHelper.Sequence(
               new SingleLabel('c'),
               new SingleLabel('b')
            ).ToList();

            Label label = new Label(expectedLabels);

            expectedLabels.Sort();

            CollectionAssert.AreEqual(expectedLabels, label.Sublabels);
            Assert.AreEqual(LabelType.Complex, label.LabelType);
        }

        [Test]
        public void CCtorTest_Simple_Ok()
        {
            SingleLabel singleLabel = new SingleLabel('b');
            IEnumerable<SingleLabel> expectedLabels = EnumerateHelper.Sequence(
               singleLabel
            );

            Label label = new Label(singleLabel);

            CollectionAssert.AreEquivalent(expectedLabels, label.Sublabels);
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
            Label label = new Label(EnumerateHelper.Sequence( new SingleLabel('b'), new SingleLabel('c')));

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
        public void NextTest_NonSimple_Fail()
        {
            Assert.Throws<InvalidOperationException>(() => new Label(EnumerateHelper.Sequence(new SingleLabel('b', 0, 1, 2), new SingleLabel('c', 0, 1, 2))).Next());
        }

        [Test]
        public void ConvertToComplexTest_Ok()
        {
            Label expectedLabel = new Label(EnumerateHelper.Sequence(new SingleLabel('b')));
            Label actualLabel = new Label(new SingleLabel('b')).ConvertToComplex();
            
            Assert.AreEqual(expectedLabel, actualLabel);
        }

        [Test]
        public void ConvertToComplexTest_NonSimple_Fail()
        {
            Assert.Throws<InvalidOperationException>(() => new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))).ConvertToComplex());
        }

        [Test]
        public void ExtractSingleLabelTest_Ok()
        {
            SingleLabel actualSingleLabel = new Label(new SingleLabel('b')).ExtractSingleLabel();
            SingleLabel expectedSingleLabel = new SingleLabel('b');

            Assert.AreEqual(expectedSingleLabel, actualSingleLabel);
        }

        [Test]
        public void ExtractSingleLabelTest_NonSimple_Fail()
        {
            Assert.Throws<InvalidOperationException>(() => new Label(EnumerateHelper.Sequence(new SingleLabel('b'), new SingleLabel('c'))).ExtractSingleLabel());
        }
    }
}

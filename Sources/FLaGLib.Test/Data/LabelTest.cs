using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using FLaGLib.Data;
using FLaGLib.Test.Helpers;

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
        public void Cctor_ComplexNull_Fail()
        {
            new Label((ISet<SingleLabel>)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cctor_SimpleNull_Fail()
        {
            new Label((SingleLabel)null);
        }

        [Test]
        public void CCtor_Complex_Ok()
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
        public void CCtor_Simple_Ok()
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
            Label label = new Label(new SingleLabel('b'));

            Assert.AreEqual("{b_null_null_null}", label.ToString());
        }
    }
}

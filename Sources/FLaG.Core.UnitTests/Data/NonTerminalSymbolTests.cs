using System.Collections.Immutable;
using FLaG.Core.Data;
using FLaG.Core.Data.Grammars;

namespace FLaG.Core.UnitTests.Data
{
    [TestFixture]
    public sealed class NonTerminalSymbolTests
    {
        [Test]
        public void Regression1()
        {
            IImmutableSet<NonTerminalSymbol> nonTerminals = new[]
            {
                new NonTerminalSymbol(new Label(new SingleLabel('S', 1))),
                new NonTerminalSymbol(new Label(new SingleLabel('S', 2))),
                new NonTerminalSymbol(new Label(new SingleLabel('S', 1))),
            }.ToImmutableSortedSet();

            Assert.That(nonTerminals, Has.Count.EqualTo(2));
        }

        [Test]
        public void Regression2()
        {
            NonTerminalSymbol symbol1 = new(new Label(new SingleLabel('S', 1)));
            NonTerminalSymbol symbol2 = new(new Label(new SingleLabel('S', 1)));

            Assert.That(symbol1.CompareTo(symbol2), Is.EqualTo(0));
            Assert.That(symbol2.CompareTo(symbol1), Is.EqualTo(0));
        }

        [Test]
        public void Regression3()
        {
            Label symbol1 = new(new SingleLabel('S', 1));
            Label symbol2 = new(new SingleLabel('S', 1));

            Assert.That(symbol1.CompareTo(symbol2), Is.EqualTo(0));
            Assert.That(symbol2.CompareTo(symbol1), Is.EqualTo(0));
        }

        [Test]
        public void Regression4()
        {
            SingleLabel symbol1 = new('S', 1);
            SingleLabel symbol2 = new('S', 1);

            Assert.That(symbol1.CompareTo(symbol2), Is.EqualTo(0));
            Assert.That(symbol2.CompareTo(symbol1), Is.EqualTo(0));
        }
    }
}

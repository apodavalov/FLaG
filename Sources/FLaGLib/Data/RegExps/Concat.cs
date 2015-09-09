using FLaGLib.Data.Grammars;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public class Concat : Expression, IEquatable<Concat>, IComparable<Concat>
    {
        public IReadOnlyList<Expression> Expressions
        {
            get;
            private set;
        }

        public Concat(IEnumerable<Expression> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            Expressions = new List<Expression>(expressions).AsReadOnly();

            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Concat must have at least two items.");
            }
        }

        public static bool operator ==(Concat objA, Concat objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Concat objA, Concat objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Concat objA, Concat objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Concat objA, Concat objB)
        {
            if ((object)objA == null)
            {
                if ((object)objB == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return objA.Equals(objB);
        }

        public static int Compare(Concat objA, Concat objB)
        {
            if (objA == null)
            {
                if (objB == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            return objA.CompareTo(objB);
        }

        public bool Equals(Concat other)
        {
            if (other == null)
            {
                return false;
            }

            return Expressions.SequenceEqual(other.Expressions);
        }

        public int CompareTo(Concat other)
        {
            if (other == null)
            {
                return 1;
            }

            return Expressions.SequenceCompare(other.Expressions);
        }

        public override bool Equals(object obj)
        {
            Concat concat = obj as Concat;
            return Equals(concat);
        }

        public override int GetHashCode()
        {
            return Expressions.GetSequenceHashCode();
        }

        public override bool Equals(Expression other)
        {
            Concat concat = other as Concat;
            return Equals(concat);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Concat)
            {
                return CompareTo((Concat)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }

        public override int Priority
        {
            get 
            {
                return 2;
            }
        }

        public override Expression ToRegularSet()
        {
            if (IsRegularSet)
            {
                return this;
            }

            return new Concat(Expressions.Select(e => e.ToRegularSet()));
        }

        protected override bool GetIsRegularSet()
        {
            return Expressions.All(e => e.IsRegularSet);
        }

        internal override IEnumerable<DepthData<Expression>> WalkInternal()
        {
            yield return new DepthData<Expression>(this, WalkStatus.Begin);

            foreach (Expression expression in Expressions)
            {
                foreach (DepthData<Expression> data in expression.WalkInternal())
                {
                    yield return data;
                }
            }

            yield return new DepthData<Expression>(this, WalkStatus.End);
        }

        internal override void ToString(StringBuilder builder)
        {
            bool first = true;

            foreach (Expression expression in Expressions)
            {
                if (first)
                {
                    expression.ToString(expression.Priority > Priority, builder);
                    first = false;
                }
                else
                {                                    
                    expression.ToString(expression.Priority >= Priority, builder);
                }
            }
        }

        protected override IReadOnlyList<Expression> GetDirectDependencies()
        {
            return Expressions;
        }

        protected override Grammar GenerateGrammar(GrammarType grammarType)
        {
            throw new NotSupportedException();
        }
    }
}

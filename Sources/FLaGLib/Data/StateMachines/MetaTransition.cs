using FLaGLib.Collections;
using FLaGLib.Extensions;
using System.Linq;
using System;
using System.Globalization;
using System.Text;

namespace FLaGLib.Data.StateMachines
{
    public class MetaTransition : IComparable<MetaTransition>, IEquatable<MetaTransition>
    {
        public IReadOnlySet<Label> CurrentRequiredStates
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> CurrentOptionalStates
        {
            get;
            private set;
        }

        public char Symbol
        {
            get;
            private set;
        }

        public IReadOnlySet<Label> NextStates
        {
            get;
            private set;
        }

        public MetaTransition(
            IReadOnlySet<Label> metaCurrentRequiredStates,
            IReadOnlySet<Label> metaCurrentOptionalStates, 
            char symbol,
            IReadOnlySet<Label> metaNextStates)
        {
            if (metaCurrentRequiredStates == null)
            {
                throw new ArgumentNullException("metaCurrentRequiredStates");
            }

            if (metaCurrentOptionalStates == null)
            {
                throw new ArgumentNullException("metaCurrentOptionalStates");
            }

            if (metaNextStates == null)
            {
                throw new ArgumentNullException("metaNextStates");
            }

            CurrentOptionalStates = metaCurrentOptionalStates;
            CurrentRequiredStates = metaCurrentRequiredStates;
            NextStates = metaNextStates;
            Symbol = symbol;
        }

        public static bool operator ==(MetaTransition objA, MetaTransition objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(MetaTransition objA, MetaTransition objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(MetaTransition objA, MetaTransition objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(MetaTransition objA, MetaTransition objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(MetaTransition objA, MetaTransition objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(MetaTransition objA, MetaTransition objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(MetaTransition objA, MetaTransition objB)
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

        public static int Compare(MetaTransition objA, MetaTransition objB)
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

        public override bool Equals(object obj)
        {
            MetaTransition metaTransition = obj as MetaTransition;
            return Equals(metaTransition);
        }

        public override int GetHashCode()
        {
            return
                Symbol.GetHashCode() ^
                CurrentRequiredStates.GetSequenceHashCode() ^
                CurrentOptionalStates.GetSequenceHashCode() ^
                NextStates.GetSequenceHashCode();
        }

        public bool Equals(MetaTransition other)
        {
            if (other == null)
            {
                return false;
            }

            return
                CurrentOptionalStates.SequenceEqual(other.CurrentOptionalStates) &&
                CurrentRequiredStates.SequenceEqual(other.CurrentRequiredStates) &&
                NextStates.SequenceEqual(other.NextStates) &&
                Symbol == other.Symbol;
        }

        public int CompareTo(MetaTransition other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = Symbol.CompareTo(other.Symbol);

            if (result != 0)
            {
                return result;
            }

            result = CurrentOptionalStates.SequenceCompare(other.CurrentOptionalStates);

            if (result != 0)
            {
                return result;
            }

            result = CurrentRequiredStates.SequenceCompare(other.CurrentRequiredStates);

            if (result != 0)
            {
                return result;
            }

            return NextStates.SequenceCompare(other.NextStates);
        }

        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder("δ([");

            foreach (Label l in CurrentRequiredStates)
            {
                sb.Append(l.ToString());
                sb.Append(' ');
            }

            sb.Append("q_1 ... q_");
            sb.Append(CurrentOptionalStates.Count);
            sb.Append("], ");
            sb.Append(Symbol);
            sb.Append(") -> [");

            foreach (Label l in NextStates)
            {
                sb.Append(' ');
                sb.Append(l.ToString());                
            }

            sb.Append("], q_1 ... q_");
            sb.Append(CurrentOptionalStates.Count);
            sb.Append(" :");

            foreach (Label l in CurrentOptionalStates)
            {
                sb.Append(' ');
                sb.Append(l.ToString());
            }

            return sb.ToString();
        }
    }
}

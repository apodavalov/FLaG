using FLaGLib.Collections;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLaGLib.Data
{
    public class Label : IComparable<Label>, IEquatable<Label>
    {
        public IReadOnlySet<SingleLabel> Sublabels
        {
            get;
            private set;        
        }

        public LabelType LabelType
        {
            get;
            private set;
        }

        public Label(IEnumerable<SingleLabel> sublabels)
        {
            if (sublabels == null)
            {
                throw new ArgumentNullException("sublabels");
            }

            if (!sublabels.Any())
            {
                throw new ArgumentException("Parameter sublabels contains no labels.");
            }

            foreach (SingleLabel label in sublabels)
            {
                if (label == null)
                {
                    throw new ArgumentException("One of the sublabels is null.");
                }
            }

            Sublabels = sublabels.ToSortedSet().AsReadOnly();

            LabelType = LabelType.Complex;
        }

        public Label(SingleLabel singleLabel)
        {
            if (singleLabel == null)
            {
                throw new ArgumentNullException("singleLabel");
            }

            Sublabels = new SortedSet<SingleLabel>(new SingleLabel[] { singleLabel } ).AsReadOnly();
            LabelType = LabelType.Simple;
        }

        public static bool operator ==(Label objA, Label objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Label objA, Label objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Label objA, Label objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Label objA, Label objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Label objA, Label objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Label objA, Label objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Label objA, Label objB)
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

        public static int Compare(Label objA, Label objB)
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
            Label label = obj as Label;
            return Equals(label);
        }

        public override int GetHashCode()
        {
            int hash = LabelType.GetHashCode();

            foreach (SingleLabel label in Sublabels)
            {
                hash ^= label.GetHashCode();
            }

            return hash;
        }

        public bool Equals(Label other)
        {
            if (other == null)
            {
                return false;
            }

            if (!LabelType.Equals(other.LabelType))
            {
                return false;
            }

            return Sublabels.SequenceEqual(other.Sublabels);
        }

        public int CompareTo(Label other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = LabelType.CompareTo(other.LabelType);

            if (result != 0)
            {
                return result;
            }

            return Sublabels.SequenceCompare(other.Sublabels);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (LabelType == LabelType.Complex)
            {
                builder.Append("[");
            }

            foreach (SingleLabel label in Sublabels)
            {
                builder.Append("{").Append(label).Append("}");
            }


            if (LabelType == LabelType.Complex)
            {
                builder.Append("]");
            }

            return builder.ToString();
        }

        public Label Next()
        {
            if (LabelType != LabelType.Simple)
            {
                throw new InvalidOperationException("Cannot produce next label for non simple label type.");
            }

            SingleLabel label = Sublabels.Single();

            return new Label(label.Next());
        }

        public Label ConvertToComplex()
        {
            if (LabelType != LabelType.Simple)
            {
                throw new InvalidOperationException("Cannot convert label to complex for non simple label type.");
            }

            return new Label(new HashSet<SingleLabel>(Sublabels));
        }

        public SingleLabel ExtractSingleLabel()
        {
            if (LabelType != LabelType.Simple)
            {
                throw new InvalidOperationException("Cannot extract single label from non simple label type.");
            }

            return Sublabels.Single();
        }
    }
}

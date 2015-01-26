﻿using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using FLaGLib.Extensions;

namespace FLaGLib.Data.RegExps
{
    public class Union : Expression, IEquatable<Union>, IComparable<Union>
    {
        public IReadOnlySet<Expression> Expressions
        {
            get;
            private set;
        }

        public Union(IEnumerable<Expression> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            Expressions = new SortedSet<Expression>(expressions).AsReadOnly();

            if (Expressions.Count < 2)
            {
                throw new ArgumentException("Union must have at least two items.");
            }
        }

        public static bool operator ==(Union objA, Union objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Union objA, Union objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Union objA, Union objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Union objA, Union objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Union objA, Union objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Union objA, Union objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Union objA, Union objB)
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

        public static int Compare(Union objA, Union objB)
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

        public bool Equals(Union other)
        {
            if (other == null)
            {
                return false;
            }

            return Expressions.SequenceEqual(other.Expressions);
        }

        public int CompareTo(Union other)
        {
            if (other == null)
            {
                return 1;
            }

            return Expressions.SequenceCompare(other.Expressions);
        }

        public override bool Equals(object obj)
        {
            Union union = obj as Union;
            return Equals(union);
        }

        public override int GetHashCode()
        {
            return Expressions.GetSequenceHashCode();
        }

        public override bool Equals(Expression other)
        {
            Union union = other as Union;
            return Equals(union);
        }

        public override int CompareTo(Expression other)
        {
            if (other == null || other is Union)
            {
                return CompareTo((Union)other);
            }

            return string.Compare(GetType().FullName, other.GetType().FullName);
        }
    }
}

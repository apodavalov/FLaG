using FLaGLib.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace FLaGLib.Data.RegExps
{
    public abstract class Expression : IEquatable<Expression>, IComparable<Expression>
    {
        internal Expression() 
        {
            _IsRegularSet = new Lazy<bool>(GetIsRegularSet);
        }

        public static bool operator ==(Expression objA, Expression objB)
        {
            return Equals(objA, objB);
        }

        public static bool operator !=(Expression objA, Expression objB)
        {
            return !Equals(objA, objB);
        }

        public static bool operator <(Expression objA, Expression objB)
        {
            return Compare(objA, objB) < 0;
        }

        public static bool operator >(Expression objA, Expression objB)
        {
            return Compare(objA, objB) > 0;
        }

        public static bool operator >=(Expression objA, Expression objB)
        {
            return Compare(objA, objB) > -1;
        }

        public static bool operator <=(Expression objA, Expression objB)
        {
            return Compare(objA, objB) < 1;
        }

        public static bool Equals(Expression objA, Expression objB)
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

        public static int Compare(Expression objA, Expression objB)
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

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();

        public abstract bool Equals(Expression other);

        public abstract int CompareTo(Expression other);

        public abstract int Priority { get; }

        public IReadOnlyList<WalkData<Expression>> Walk(int startIndex)
        {
            WalkDataLabel<DepthData<Expression>>[] depthData = 
               WalkInternal().Select(data => new WalkDataLabel<DepthData<Expression>>(false,0,data)).ToArray();

            int maxDepth = 0;
            int currentDepth = 0;

            foreach (WalkDataLabel<DepthData<Expression>> data in depthData)
            {
                if (data.Value.Status == WalkStatus.Begin)
                {
                    currentDepth++;
                }
                else
                {
                    currentDepth--;
                }

                if (maxDepth < currentDepth)
                {
                    maxDepth = currentDepth;
                }
            }

            int currentIndex = startIndex;
          
            while (maxDepth > 0)
            {
                WalkDataLabel<DepthData<Expression>> prev = null;

                foreach (WalkDataLabel<DepthData<Expression>> data in depthData)
                {
                    if (prev != null)
                    {
                        if (prev.Value.Status == WalkStatus.Begin && data.Value.Status == WalkStatus.End && !data.Marked)
                        {
                            prev.Marked = true;
                            data.Marked = true;
                            prev.Index = currentIndex;
                            data.Index = currentIndex;

                            currentIndex++;

                            prev = null;
                        }
                    }

                    if (!data.Marked)
                    {
                        prev = data;
                    }
                }

                maxDepth--;
            }

            return depthData.Select(data => new WalkData<Expression>(data.Value.Status, data.Index, data.Value.Value)).ToList().AsReadOnly();
        }

        public abstract Expression ToRegularSet();

        private Lazy<bool> _IsRegularSet;
        public bool IsRegularSet
        {
            get
            {
                return _IsRegularSet.Value;
            }
        } 

        protected abstract bool GetIsRegularSet();
        
        internal abstract IEnumerable<DepthData<Expression>> WalkInternal();

        internal abstract void ToString(StringBuilder builder);

        internal void ToString(bool needBrackets, StringBuilder builder)
        {
            if (needBrackets)
            {
                builder.Append('(');
            }

            ToString(builder);

            if (needBrackets)
            {
                builder.Append(')');
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            ToString(sb);

            return sb.ToString();
        }
    }
}

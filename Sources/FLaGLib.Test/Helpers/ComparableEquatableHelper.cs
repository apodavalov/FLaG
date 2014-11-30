﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FLaGLib.Test.Helpers
{
    static class ComparableEquatableHelper
    {
        public static void TestEquals<T>(IEnumerable<Tuple<T, T, int>> expectations) where T : IEquatable<T>
        {
            foreach (Tuple<T, T, int> expectation in expectations)
            {
                Type typeofT = typeof(T);

                MethodInfo method = GetStaticMethod(typeofT, "Equals", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 == 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 == 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                method = GetStaticMethod(typeofT, "op_Equality", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 == 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 == 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                method = GetStaticMethod(typeofT, "op_Inequality", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 != 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 != 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                if (expectation.Item1 != null)
                {
                    Assert.AreEqual(expectation.Item3 == 0, expectation.Item1.Equals(expectation.Item2));
                    Assert.AreEqual(expectation.Item3 == 0, expectation.Item1.Equals((object)expectation.Item2));
                }

                if (expectation.Item2 != null)
                {
                    Assert.AreEqual(expectation.Item3 == 0, expectation.Item2.Equals(expectation.Item1));
                    Assert.AreEqual(expectation.Item3 == 0, expectation.Item2.Equals((object)expectation.Item1));
                }
            }
        }

        public static void TestCompare<T>(IEnumerable<Tuple<T, T, int>> expectations) where T : IComparable<T>
        {
            foreach (Tuple<T, T, int> expectation in expectations)
            {
                Type typeofT = typeof(T);

                MethodInfo method = GetStaticMethod(typeofT, "op_GreaterThanOrEqual", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 >= 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 <= 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                method = GetStaticMethod(typeofT, "op_LessThanOrEqual", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 <= 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 >= 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                 method = GetStaticMethod(typeofT, "op_GreaterThan", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 > 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 < 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                method = GetStaticMethod(typeofT, "op_LessThan", new Type[] { typeofT, typeofT }, typeof(bool));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3 < 0, (bool)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(expectation.Item3 > 0, (bool)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                method = GetStaticMethod(typeofT, "Compare", new Type[] { typeofT, typeofT }, typeof(int));

                if (method != null)
                {
                    Assert.AreEqual(expectation.Item3, (int)method.Invoke(null, new object[] { expectation.Item1, expectation.Item2 }));
                    Assert.AreEqual(-expectation.Item3, (int)method.Invoke(null, new object[] { expectation.Item2, expectation.Item1 }));
                }

                if (expectation.Item1 != null)
                {
                    Assert.AreEqual(expectation.Item3, expectation.Item1.CompareTo(expectation.Item2));
                }

                if (expectation.Item2 != null)
                {
                    Assert.AreEqual(-expectation.Item3, expectation.Item2.CompareTo(expectation.Item1));
                }
            }
        }

        private static MethodInfo GetStaticMethod(Type type, string name, Type[] types, Type returnType)
        {
            MethodInfo method = type.GetMethod(name, types);

            if (method != null && !method.IsStatic)
            {
                method = null;
            }

            if (method != null && method.ReturnType != returnType)
            {
                method = null;
            }

            return method;
        }
    }
}

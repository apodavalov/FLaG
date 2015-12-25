using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Languages = FLaGLib.Data.Languages;
using RegExps = FLaGLib.Data.RegExps;

namespace FLaG.IO.Output
{
    public static class StreamWriterExtensions
    {
        public static void WriteExpressionEx(this StreamWriter writer, RegExps.Expression expression)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            IReadOnlyList<RegExps.DependencyCollection> dependencyMap = expression.DependencyMap;

            WriteExpressionEx(writer, dependencyMap, dependencyMap.Count - 1);
        }

        private static void WriteExpressionEx(StreamWriter writer, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            writer.Write(@"\underbrace{");

            RegExps.Expression expression = dependencyMap[index].Expression;

            switch (expression.ExpressionType)
            {
                case RegExps.ExpressionType.Concat:
                    WriteConcatEx(writer, (RegExps.Concat)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.BinaryConcat:
                    WriteBinaryConcatEx(writer, (RegExps.BinaryConcat)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.Union:
                    WriteUnionEx(writer, (RegExps.Union)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.BinaryUnion:
                    WriteBinaryUnionEx(writer, (RegExps.BinaryUnion)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.Symbol:
                    WriteSymbolEx(writer, (RegExps.Symbol)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.Iteration:
                    WriteIterationEx(writer, (RegExps.Iteration)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.ConstIteration:
                    WriteConstIterationEx(writer, (RegExps.ConstIteration)expression, dependencyMap, index);
                    break;
                case RegExps.ExpressionType.Empty:
                    WriteEmptyEx(writer, (RegExps.Empty)expression, dependencyMap, index);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The expression type {0} is not supported.", expression.ExpressionType));
            }

            writer.Write(@"}_\text{");
            writer.WriteLatex((index + 1).ToString());
            writer.Write(@"}");
        }

        private static void WriteEmptyEx(StreamWriter writer, RegExps.Empty empty, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            writer.Write(@"{\varepsilon}");
        }

        private static void WriteConstIterationEx(StreamWriter writer, RegExps.ConstIteration constIteration, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            bool needBrackets = constIteration.Expression.Priority >= constIteration.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteExpressionEx(writer, dependencyMap, dependencyMap[index].Single());

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");
            writer.WriteLatex(constIteration.IterationCount.ToString());
            writer.Write("}");
        }

        private static void WriteIterationEx(StreamWriter writer, RegExps.Iteration iteration, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            bool needBrackets = iteration.Expression.Priority >= iteration.Priority;

            if (needBrackets)
            {
                writer.Write(@"\left(");
            }

            WriteExpressionEx(writer, dependencyMap, dependencyMap[index].Single());

            if (needBrackets)
            {
                writer.Write(@"\right)");
            }

            writer.Write("^");
            writer.Write("{");

            if (iteration.IsPositive)
            {
                writer.Write("+");
            }
            else
            {
                writer.Write("*");
            }

            writer.Write("}");
        }

        private static void WriteSymbolEx(StreamWriter writer, RegExps.Symbol symbol, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            writer.WriteLatex(symbol.Character.ToString());
        }

        private static void WriteBinaryUnionEx(StreamWriter writer, RegExps.BinaryUnion binaryUnion, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            WriteExpressionsEx(writer, dependencyMap, index, " + ");
        }

        private static void WriteBinaryConcatEx(StreamWriter writer, RegExps.BinaryConcat binaryConcat, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            WriteExpressionsEx(writer, dependencyMap, index, @" \cdot ");
        }

        private static void WriteExpressionsEx(StreamWriter writer, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index, string separator)
        {
            RegExps.DependencyCollection dependencies = dependencyMap[index];

            bool first = true;

            foreach (int i in dependencies)
            {
                RegExps.DependencyCollection currentDependencies = dependencyMap[i];

                bool needBrackets;

                if (first)
                {
                    needBrackets = currentDependencies.Expression.Priority > dependencies.Expression.Priority;
                    first = false;
                }
                else
                {
                    needBrackets = currentDependencies.Expression.Priority >= dependencies.Expression.Priority;
                    writer.Write(separator);
                }

                if (needBrackets)
                {
                    writer.Write(@"\left(");
                }

                WriteExpressionEx(writer, dependencyMap, i);

                if (needBrackets)
                {
                    writer.Write(@"\right)");
                }
            }
        }

        private static void WriteUnionEx(StreamWriter writer, RegExps.Union union, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            throw new NotSupportedException();
        }

        private static void WriteConcatEx(StreamWriter writer, RegExps.Concat concat, IReadOnlyList<RegExps.DependencyCollection> dependencyMap, int index)
        {
            throw new NotSupportedException();
        }

        public static void WriteExpression(this StreamWriter writer, RegExps.Expression expression, bool writeDots = false)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            switch (expression.ExpressionType)
            {
                case RegExps.ExpressionType.Concat:
                    WriteConcat(writer, (RegExps.Concat)expression, writeDots);
                    break;
                case RegExps.ExpressionType.BinaryConcat:
                    WriteBinaryConcat(writer, (RegExps.BinaryConcat)expression, writeDots);
                    break;
                case RegExps.ExpressionType.Union:
                    WriteUnion(writer, (RegExps.Union)expression, writeDots);
                    break;
                case RegExps.ExpressionType.BinaryUnion:
                    WriteBinaryUnion(writer, (RegExps.BinaryUnion)expression, writeDots);
                    break;
                case RegExps.ExpressionType.Symbol:
                    WriteSymbol(writer, (RegExps.Symbol)expression);
                    break;
                case RegExps.ExpressionType.Iteration:
                    WriteIteration(writer, (RegExps.Iteration)expression, writeDots);
                    break;
                case RegExps.ExpressionType.ConstIteration:
                    WriteConstIteration(writer, (RegExps.ConstIteration)expression, writeDots);
                    break;
                case RegExps.ExpressionType.Empty:
                    WriteEmpty(writer, (RegExps.Empty)expression);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The expression type {0} is not supported.", expression.ExpressionType));
            }
        }

        public static void WriteSymbol(StreamWriter writer, RegExps.Symbol symbol)
        {
            writer.WriteLatex(symbol.Character.ToString());
        }

        public static void WriteConstIteration(StreamWriter writer, RegExps.ConstIteration constIteration, bool writeDots)
        {
            bool needBrackets = constIteration.Expression.Priority >= constIteration.Priority;

            if (needBrackets)
            {
                writer.Write("(");
            }

            WriteExpression(writer, constIteration.Expression, writeDots);

            if (needBrackets)
            {
                writer.Write(")");
            }

            writer.Write("^");
            writer.Write("{");
            writer.WriteLatex(constIteration.IterationCount.ToString());
            writer.Write("}");
        }

        public static void WriteIteration(StreamWriter writer, RegExps.Iteration iteration, bool writeDots)
        {
            bool needBrackets = iteration.Expression.Priority >= iteration.Priority;

            if (needBrackets)
            {
                writer.Write("(");
            }

            WriteExpression(writer, iteration.Expression, writeDots);

            if (needBrackets)
            {
                writer.Write(")");
            }

            writer.Write("^");
            writer.Write("{");

            if (iteration.IsPositive)
            {
                writer.Write("+");
            }
            else
            {
                writer.Write("*");
            }

            writer.Write("}");
        }

        public static void WriteEmpty(StreamWriter writer, RegExps.Empty empty)
        {
            writer.Write(@"{\varepsilon}");
        }

        public static void WriteBinaryUnion(StreamWriter writer, RegExps.BinaryUnion binaryUnion, bool writeDots)
        {
            ISet<RegExps.Expression> visitedExpression = new HashSet<RegExps.Expression>();
            WriteExpressions(writer, RegExps.UnionHelper.Iterate(visitedExpression, binaryUnion), " + ", binaryUnion.Priority, writeDots);
        }

        public static void WriteUnion(StreamWriter writer, RegExps.Union union, bool writeDots)
        {
            ISet<RegExps.Expression> visitedExpression = new HashSet<RegExps.Expression>();
            WriteExpressions(writer, RegExps.UnionHelper.Iterate(visitedExpression, union), " + ", union.Priority, writeDots);
        }

        public static void WriteBinaryConcat(StreamWriter writer, RegExps.BinaryConcat binaryConcat, bool writeDots)
        {
            WriteExpressions(writer, RegExps.ConcatHelper.Iterate(binaryConcat), string.Empty, binaryConcat.Priority, writeDots);
        }

        public static void WriteConcat(StreamWriter writer, RegExps.Concat concat, bool writeDots)
        {
            WriteExpressions(writer, RegExps.ConcatHelper.Iterate(concat), writeDots ? @" \cdot ": string.Empty, concat.Priority, writeDots);
        }

        private static void WriteExpressions(StreamWriter writer, IEnumerable<RegExps.Expression> expressions, string separator, int priority, bool writeDots)
        {
            bool first = true;

            foreach (RegExps.Expression expression in expressions)
            {
                bool needBrackets;

                if (first)
                {
                    needBrackets = expression.Priority > priority;
                    first = false;
                }
                else
                {
                    needBrackets = expression.Priority >= priority;
                    writer.Write(separator);
                }

                if (needBrackets)
                {
                    writer.Write("(");
                }

                WriteExpression(writer, expression, writeDots);

                if (needBrackets)
                {
                    writer.Write(")");
                }
            }
        }

        public static void WriteLanguage(this StreamWriter writer, Languages.Entity entity)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            writer.Write(@"\left\{");

            WriteEntity(writer, entity);
            WriteVariables(writer, entity.Variables);

            writer.Write(@"\right\}");
        }

        private static void WriteEntity(StreamWriter writer, Languages.Entity entity)
        {
            switch (entity.EntityType)
            {
                case Languages.EntityType.Concat:
                    WriteConcat(writer, (Languages.Concat)entity);
                    break;
                case Languages.EntityType.Union:
                    WriteUnion(writer, (Languages.Union)entity);
                    break;
                case Languages.EntityType.Symbol:
                    WriteSymbol(writer, (Languages.Symbol)entity);
                    break;
                case Languages.EntityType.Degree:
                    WriteDegree(writer, (Languages.Degree)entity);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The entity type {0} is not supported.", entity.EntityType));
            }
        }

        private static void WriteSymbol(StreamWriter writer, Languages.Symbol symbol)
        {
            writer.WriteLatex(symbol.Character.ToString());
        }

        private static void WriteDegree(StreamWriter writer, Languages.Degree degree)
        {
            bool needBrackets = degree.Entity.Priority >= degree.Priority;

            if (needBrackets)
            {
                writer.Write("(");
            }

            WriteEntity(writer, degree.Entity);

            if (needBrackets)
            {
                writer.Write(")");
            }

            writer.Write("^");
            writer.Write("{");
            WriteExponent(writer, degree.Exponent);
            writer.Write("}");
        }

        private static void WriteUnion(StreamWriter writer, Languages.Union union)
        {
            WriteEntities(writer, union.EntityCollection, ",", union.Priority);
        }

        private static void WriteConcat(StreamWriter writer, Languages.Concat union)
        {
            WriteEntities(writer, union.EntityCollection, string.Empty, union.Priority);
        }

        private static void WriteEntities(StreamWriter writer, IEnumerable<Languages.Entity> entities, string separator, int priority)
        {
            bool first = true;

            foreach (Languages.Entity entity in entities)
            {
                bool needBrackets;

                if (first)
                {
                    needBrackets = entity.Priority > priority;
                    first = false;
                }
                else
                {
                    needBrackets = entity.Priority >= priority;
                    writer.Write(separator);
                }

                if (needBrackets)
                {
                    writer.Write("(");
                }

                WriteEntity(writer, entity);

                if (needBrackets)
                {
                    writer.Write(")");
                }
            }
        }

        private static void WriteExponent(StreamWriter writer, Languages.Exponent exponent)
        {
            switch (exponent.ExponentType)
            {
                case Languages.ExponentType.Quantity:
                    WriteQuantity(writer, (Languages.Quantity)exponent);
                    break;
                case Languages.ExponentType.Variable:
                    WriteVariable(writer, (Languages.Variable)exponent);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("The exponent type {0} is not supported.", exponent.ExponentType));
            }
        }

        private static void WriteVariable(StreamWriter writer, Languages.Variable variable)
        {
            writer.WriteLatex(variable.Name.ToString());
        }

        private static void WriteQuantity(StreamWriter writer, Languages.Quantity quantity)
        {
            writer.WriteLatex(quantity.Count.ToString());
        }

        private static void WriteVariables(StreamWriter writer, IReadOnlySet<Languages.Variable> variables)
        {
            if (!variables.Any())
            {
                return;
            }
            
            writer.Write(@" \mid \forall ");

            foreach (Languages.Variable variable in variables)
            {
                writer.WriteLatex(variable.Name.ToString());
                switch (variable.Sign)
                {
                    case Languages.Sign.MoreThan:
                        writer.Write(@" > ");
                        break;
                    case Languages.Sign.MoreThanOrEqual:
                        writer.Write(@" \geq ");
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Sign {0} is not supported.",variable.Sign));
                }

                writer.WriteLatex(variable.Number.ToString());
                writer.Write(@", ");
            }

            writer.Write(@"\text{где } ");

            writer.Write(string.Join(", ", variables.Select(v => Escape(v.Name.ToString()))));

            writer.Write(@" \text{ --- целые}");
        }

        public static void WriteLatex(this StreamWriter writer, string value)
        {
            writer.Write(Escape(value));
        }

        public static void WriteLineLatex(this StreamWriter writer, string value)
        {
            writer.WriteLine(Escape(value));
        }

        private static string Escape(string value)
        {
            if (value == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                switch (c)
                {
                    case '#':
                        sb.Append(@"\#");
                        break;
                    case '$':
                        sb.Append(@"\$");
                        break;
                    case '%':
                        sb.Append(@"\%");
                        break;
                    case '^':
                        sb.Append(@"\textasciicircum{}");
                        break;
                    case '&':
                        sb.Append(@"\&");
                        break;
                    case '_':
                        sb.Append(@"\_");
                        break;
                    case '{':
                        sb.Append(@"\{");
                        break;
                    case '}':
                        sb.Append(@"\}");
                        break;
                    case '~':
                        sb.Append(@"\~{}");
                        break;
                    case '\\':
                        sb.Append(@"\textbackslash{}");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }                
            }

            return sb.ToString();
        }
    }
}

using FLaGLib.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Languages = FLaGLib.Data.Languages;

namespace FLaG.IO.Output
{
    public static class StreamWriterExtensions
    {
        public static void WriteLanguage(this StreamWriter writer, Languages.Entity entity)
        {
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

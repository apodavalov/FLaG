using System.IO;
using System.Text;

namespace FLaG.IO.Output
{
    public static class StreamWriterExtensions
    {
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

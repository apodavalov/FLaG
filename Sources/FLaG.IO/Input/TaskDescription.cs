using FLaGLib.Collections;
using FLaGLib.Data.Languages;
using FLaGLib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace FLaG.IO.Input
{
    public class TaskDescription
    {
        private const string _TaskElementName = "task";
        private const string _VariablesElementName = "variables";
        private const string _VariableElementName = "variable";
        private const string _LanguageElementName = "language";
        private const string _InfoElementName = "info";
        private const string _AuthorElementName = "author";
        private const string _VariantElementName = "variant";

        private const string _FirstNameAttributeName = "firstname";
        private const string _SecondNameAttributeName = "secondname";
        private const string _LastNameAttributeName = "lastname";
        private const string _GroupAttributeName = "group";

        private const string _ConcatElementName = "concat";
        private const string _UnionElementName = "union";
        private const string _DegreeElementName = "degree";
        private const string _SymbolElementName = "symbol";

        private const string _BaseElementName = "base";
        private const string _ExpElementName = "exp";

        private const string _NumberElementName = "number";
        private const string _VrefElementName = "vref";

        private const string _NumElementName = "num";
        private const string _SignElementName = "sign";
        private const string _NameElementName = "name";

        private const string _ElementIsNotSupportedTemplate = "Element {0} is not supported.";

        private const string _SchemaPath = "FLaG.IO.Input.lang.xsd";

        public IReadOnlySet<Variable> Variables
        {
            get;
            private set;
        }

        public Entity Language
        {
            get;
            private set;
        }

        public AuthorDescription Author
        {
            get;
            private set;
        }

        public string Variant
        {
            get;
            private set;
        }

        public TaskDescription(Entity language, AuthorDescription author, IEnumerable<Variable> variables, string variant)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            if (variant == null)
            {
                throw new ArgumentNullException(nameof(variant));
            }

            Variables = variables.ToSortedSet().AsReadOnly();

            if (Variables.AnyNull())
            {
                throw new ArgumentException("At least one variable is null.", nameof(variables));
            }

            Language = language;
            Author = author;
            Variant = variant;
        }

        private static TaskDescription LoadFromStream(XmlReader reader)
        {
            ISet<Variable> variables = new HashSet<Variable>();
            Entity language = null;
            AuthorDescription author = null;
            string variant = null;

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            while (!reader.IsStartElement(_TaskElementName))
            {
                reader.Read();
            }

            reader.ReadStartElement(_TaskElementName);

            while (!reader.IsStartElement(_VariablesElementName))
            {
                reader.Read();
            }

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement(_VariablesElementName);

            while (reader.IsStartElement(_VariableElementName))
            {
                variables.Add(LoadVariable(reader));
            }

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement(_LanguageElementName);

            if (reader.IsStartElement())
            {
                language = LoadEntity(reader, variables.ToDictionary(v => v.Name).AsReadOnly());
            }

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            reader.ReadStartElement(_InfoElementName);

            while (reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case _AuthorElementName:
                        author = LoadAuthor(reader);
                        break;
                    case _VariantElementName:
                        variant = LoadVariant(reader);
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Element type {0} is not supported.", reader.Name));
                }
            }

            reader.ReadEndElement();

            reader.ReadEndElement();

            return new TaskDescription(language, author, variables, variant);
        }

        private static string LoadVariant(XmlReader reader)
        {
            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement(_VariantElementName);
            string variant = reader.ReadContentAsString();

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            return variant;
        }

        private static AuthorDescription LoadAuthor(XmlReader reader)
        {
            bool isEmpty = reader.IsEmptyElement;
            string firstName = null;
            string secondName = null;
            string lastName = null;
            string group = null;

            if (reader.HasAttributes)
                while (reader.MoveToNextAttribute())
                    switch (reader.Name)
                    {
                        case _FirstNameAttributeName:
                            firstName = reader.Value;
                            break;
                        case _SecondNameAttributeName:
                            secondName = reader.Value;
                            break;
                        case _LastNameAttributeName:
                            lastName = reader.Value;
                            break;
                        case _GroupAttributeName:
                            group = reader.Value;
                            break;
                    }

            reader.ReadStartElement(_AuthorElementName);

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            return new AuthorDescription(firstName, secondName, lastName, group);
        }

        public static TaskDescription LoadFromStream(Stream stream)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream schema = assembly.GetManifestResourceStream(_SchemaPath))
            {
                XmlSchema xmlSchema = XmlSchema.Read(schema, null);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints;
                settings.IgnoreComments = true;
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
                settings.Schemas.Add(xmlSchema);

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    return LoadFromStream(reader);
                }
            }
        }

        public static TaskDescription Load(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadFromStream(stream);
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }

        private static Entity LoadEntity(XmlReader reader, IReadOnlyDictionary<char,Variable> variables)
        {
            while (!reader.IsStartElement())
            {
                reader.Read();
            } 

            switch (reader.Name)
            {
                case _ConcatElementName:
                    return LoadConcat(reader, variables);
                case _UnionElementName:
                    return LoadUnion(reader, variables);
                case _DegreeElementName:
                    return LoadDegree(reader, variables);
                case _SymbolElementName:
                    return LoadSymbol(reader, variables);
                default:
                    throw new InvalidOperationException(string.Format("Entity type {0} is not supported.", reader.Name));
            }
        }

        private static Symbol LoadSymbol(XmlReader reader, IReadOnlyDictionary<char, Variable> variables)
        {
            while (!reader.IsStartElement(_SymbolElementName))
            {
                reader.Read();
            }

            reader.ReadStartElement();

            char value = reader.ReadContentAsString()[0];

            reader.ReadEndElement();

            return new Symbol(value);
        }

        private static Degree LoadDegree(XmlReader reader, IReadOnlyDictionary<char, Variable> variables)
        {
            Entity baseEntity = null;
            Exponent exponent = null;

            while (!reader.IsStartElement(_DegreeElementName))
            {
                reader.Read();
            }

            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                string name = reader.Name;

                reader.ReadStartElement();

                if (reader.IsStartElement())
                {
                    switch (name)
                    {
                        case _BaseElementName:
                            baseEntity = LoadEntity(reader, variables);
                            break;
                        case _ExpElementName:
                            exponent = LoadExponent(reader, variables);
                            break;
                        default:
                            throw new InvalidOperationException(string.Format(_ElementIsNotSupportedTemplate, name));
                    }
                }

                reader.ReadEndElement();
            }

            reader.ReadEndElement();

            return new Degree(baseEntity, exponent);
        }

        private static Exponent LoadExponent(XmlReader reader, IReadOnlyDictionary<char, Variable> variables)
        {
            while (!reader.IsStartElement())
            {
                reader.Read();
            }

            switch (reader.Name)
            {
                case _NumberElementName:
                    return LoadQuantity(reader);
                case _VrefElementName:
                    return LoadVref(reader, variables);
                default:
                    throw new InvalidOperationException(string.Format("Exponent type {0} is not supported.", reader.Name));
            }
        }

        private static Variable LoadVref(XmlReader reader, IReadOnlyDictionary<char, Variable> variables)
        {
            reader.ReadStartElement();
            char variableName = reader.ReadContentAsString()[0];
            reader.ReadEndElement();

            return variables[variableName];
        }

        private static Quantity LoadQuantity(XmlReader reader)
        {
            while (!reader.IsStartElement(_NumberElementName))
            {
                reader.Read();
            }

            reader.ReadStartElement();

            int value = reader.ReadContentAsInt();

            reader.ReadEndElement();

            return new Quantity(value);
        }

        private static Union LoadUnion(XmlReader reader, IReadOnlyDictionary<char, Variable> variables)
        {
            ISet<Entity> entityCollection = new HashSet<Entity>();

            while (!reader.IsStartElement(_UnionElementName))
            {
                reader.Read();
            }

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement(_UnionElementName);

            while (reader.IsStartElement())
            {
                entityCollection.Add(LoadEntity(reader, variables));
            }

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            return new Union(entityCollection);
        }

        private static Concat LoadConcat(XmlReader reader, IReadOnlyDictionary<char, Variable> variables)
        {
            IList<Entity> entityCollection = new List<Entity>();

            while (!reader.IsStartElement(_ConcatElementName))
            {
                reader.Read();
            }

            bool isEmpty = reader.IsEmptyElement;

            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                entityCollection.Add(LoadEntity(reader, variables));
            }

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            return new Concat(entityCollection);
        }

        private static Variable LoadVariable(XmlReader reader)
        {
            char name = '\0';
            Sign sign = Sign.MoreThan;
            int num = 0;

            while (!reader.IsStartElement(_VariableElementName))
            {
                reader.Read();
            }

            bool isEmpty = reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                reader.MoveToFirstAttribute();

                do
                {
                    switch (reader.Name)
                    {
                        case _NameElementName:
                            name = reader.Value[0];
                            break;
                        case _SignElementName:
                            if (reader.Value == ">")
                            {
                                sign = Sign.MoreThan;
                            }
                            else
                            {
                                sign = Sign.MoreThanOrEqual;
                            }
                            break;
                        case _NumElementName:
                            num = int.Parse(reader.Value);
                            break;
                        default:
                            throw new InvalidOperationException(string.Format(_ElementIsNotSupportedTemplate, reader.Name));
                    }
                } while (reader.MoveToNextAttribute());
            }

            reader.ReadStartElement();

            if (!isEmpty)
            {
                reader.ReadEndElement();
            }

            return new Variable(name, sign, num);
        }
    }
}

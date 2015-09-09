using FLaG.Output;
using System;
using System.Xml;

namespace FLaG.Data
{
    class Variable : Quantity, IComparable<Variable>
    {
        public Variable() 
            : base()
        {

        }
		
		public override Quantity DeepClone()
		{
			Variable v = new Variable();
			
			v.Name = Name;
			v.Num = Num;
			v.Sign = Sign;
			
			return v;
		}

        public Variable(XmlReader reader) 
            : this()
        {
            while (!reader.IsStartElement("variable")) reader.Read();

            bool isEmpty = reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                reader.MoveToFirstAttribute();

                do
                {
                    switch (reader.Name)
                    {
                        case "name":
                            Name = reader.Value[0];
                            break;
                        case "sign":
                            if (reader.Value == ">")
                                Sign = SignEnum.MoreThanZero;
                            else
                                Sign = SignEnum.MoreOrEqualZero;
                            break;
                        case "num":
                            Num = int.Parse(reader.Value);
                            break;
                    }
                } while (reader.MoveToNextAttribute());
            }

            reader.ReadStartElement();

            if (!isEmpty)
                reader.ReadEndElement();
        }

        public char Name
        {
            get;
            set;
        }

        public int Num
        {
            get;
            set;
        }

        public SignEnum Sign
        {
            get;
            set;
        }

        public int CompareTo(Variable other)
        {
            if (Name < other.Name)
                return -1;
            else if (Name > other.Name)
                return 1;
            else
                return 0;
        }

        public override void Save(Writer writer)
        {
			writer.Write(Name);
        }
		
		public override void SaveAsRegularExp(Writer writer)
		{
			throw new NotSupportedException();
		}
    }
}

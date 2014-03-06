using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend;


namespace MiniLang
{
	public class Value  : IComparable<Value>
	{
		private Object rawValue;
		public TypeName Type
		{
			get
			{
				if (rawValue is Int32)
					return TypeName.Int;
				else if (rawValue is Boolean)
					return TypeName.Bool;
				else if (rawValue is String)
					return TypeName.String;
				else
					throw new SystemException();
			}
		}
		public int IntValue { get { return (Int32)rawValue; } }
		public bool BoolValue { get { return (Boolean)rawValue; } }
		public string StringValue { get { return (String)rawValue; } }
		public Value(int val)
		{
			this.rawValue = val;
		}
		public Value(bool val)
		{
			this.rawValue = val;
		}
		public Value(String val)
		{
			this.rawValue = val;
		}

		public Value(TypeName typeName)
		{
			switch (typeName)
			{
				case TypeName.Int: this.rawValue = 0; break;
				case TypeName.Bool: this.rawValue = false; break;
				case TypeName.String: this.rawValue = ""; break;
			}
		}
		public override string ToString()
		{
			return rawValue is Boolean ?
				rawValue.ToString().ToLower() :
				rawValue.ToString();
		}

		public int CompareTo(Value other)
		{
			return ((IComparable)this.rawValue).CompareTo((IComparable)other.rawValue);
		}
		public static Value FromString(TypeName type, string s, Location loc)
		{
			if (type == TypeName.Int)
				try { return new Value(Convert.ToInt32(s)); }
				catch (OverflowException) { throw new CompileException("Integer out of range", loc); }
			else if (type == TypeName.Bool && (s == "true" || s == "false"))
				return new Value(s == "true");
			else if (type == TypeName.String)
				return new Value(s);
			else
				throw new InvalidOperationException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniLang.Frontend
{
	public class Location : ICloneable
	{
		public int Line { get; set; }
		public int Offset { get; set; }
		public short Column { get; set; }
		public short Length { get; set; }
		public Location(int line, short column, int offset, short length)
		{
			this.Line = line;
			this.Column = column;
			this.Offset = offset;
			this.Length = length;
		}

		public Location()
		{
			this.Line = 1;
			this.Column = 0;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public override bool Equals(object obj)
		{
			Location loc = obj as Location;
			if (loc == null)
				return false;
			return this.Line == loc.Line && this.Column == loc.Column && this.Offset == loc.Offset && this.Length == loc.Length;
		}
		public override string ToString()
		{
			return String.Format("{0}:{1}/{2}+{3}", this.Line, this.Column, this.Offset, this.Length);
		}
		public override int GetHashCode()
		{
			throw new NotImplementedException(); // shut up, compiler
		}
		public static bool operator ==(Location a, Location b)
		{
			if (System.Object.ReferenceEquals(a, b))
				return true;
			if (((object)a == null) || ((object)b == null))
				return false;
			return a.Equals(b);
		}
		public static bool operator !=(Location a, Location b)
		{
			return !(a == b);
		}

		public string ToFriendlyString()
		{
			return String.Format("line {0}, columns {1}-{2}", this.Line, this.Column, this.Column + this.Length - 1);
		}
	}
}

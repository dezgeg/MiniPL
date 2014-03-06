using MiniLang.Frontend;

namespace MiniLang
{
	public class SymbolEntry
	{
		public Location Location { get; protected internal set; }
		public TypeName Type { get; protected internal set; }
		public Value Value { get; set; }
		public bool IsForLoopIterator { get; protected internal set; }

		public SymbolEntry(TypeName type, Location loc)
		{
			this.Location = loc;
			this.Type = type;
		}
	}
}

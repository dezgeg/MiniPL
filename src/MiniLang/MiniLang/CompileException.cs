using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend;

namespace MiniLang
{
	public class CompileException : Exception, IComparable<CompileException>
	{
		public Location Location { get; protected set; }
		public String ContextMessage { get; protected set; }
		public Location ContextLocation { get; protected set; }

		public CompileException(string message, Location loc=null) : base(message)
		{
			this.Location = loc;
		}
		public CompileException(string message, Location loc, String contextMessage, Location contextLoc)
			: base(message)
		{
			this.Location = loc;
			this.ContextMessage = contextMessage;
			this.ContextLocation = contextLoc;
		}
		public string GetFriendlyMessage()
		{
			StringBuilder msg = new StringBuilder(this.Message);
			if (this.Location != null)
				msg.AppendFormat(" at {0}", this.Location.ToFriendlyString());
			if (this.ContextMessage != null)
				msg.AppendFormat("\r\nInfo: {0} at {1}", this.ContextMessage, this.ContextLocation.ToFriendlyString());
			return msg.ToString();
		}
		public int CompareTo(CompileException other)
		{
			return this.Location.Offset - other.Location.Offset;
		}
	}
}

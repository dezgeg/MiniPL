using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniLang.Frontend
{
    public class Token
    {
        public TokenType Type { get; internal set; }
        public Location Location { get; private set; }
        private String Content { get; set; }

		public Token(TokenType type, Location loc, String content)
		{
			this.Type = type;
			this.Location = loc;
			this.Content = content;
		}
		public override bool Equals(object obj)
		{
			Token other = obj as Token;
			if (other == null)
				return false;
			return this.Type == other.Type && this.Location == other.Location && this.Content == other.Content;
		}
		public override string ToString()
		{
			return String.Format("{0}:'{1}' @ {2}", this.Type, this.GetContent(), this.Location);
		}
		public override int GetHashCode()
		{
			throw new NotImplementedException(); // shut up, compiler
		}

		public string GetContent()
		{
			return Content;
		}

		public void SetContent(string p)
		{
			this.Content = p;
		}
	}
}

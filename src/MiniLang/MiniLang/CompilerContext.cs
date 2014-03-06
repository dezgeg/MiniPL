using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend;

namespace MiniLang
{
	public class CompilerContext
	{
		public bool ErrorsAreFatal { get; set; }
		public List<CompileException> Errors { get; private set; }
		public IDictionary<string, SymbolEntry> SymbolTable { get; private set; }
		public CompilerContext(bool fatalErrors)
		{
			this.Errors = new List<CompileException>();
			this.SymbolTable = new Dictionary<string, SymbolEntry>();
			this.ErrorsAreFatal = fatalErrors;
		}
		public void AddError(CompileException ce)
		{
			if (this.ErrorsAreFatal)
				throw ce; // throws out the stack trace :/
			this.Errors.Add(ce);
		}
	}
}

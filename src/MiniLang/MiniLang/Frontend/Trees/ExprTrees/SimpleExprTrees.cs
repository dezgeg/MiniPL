using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.SemanticCheck;

namespace MiniLang.Frontend.Trees
{
	public class VariableTree : ExprTree
	{
		public String Name { get; protected set; }
		public SymbolEntry SymbolEntry { get; set; }
		public VariableTree(Token variable)
		{
			this.Location = variable.Location;
			this.Name = variable.GetContent();
		}
		public override string ToString()
		{
			return "Var:" + this.Name;
		}
		public override TExprRet Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			return itv.visitExpr(this);
		}
	}
	public class LiteralTree : ExprTree
	{
		public Value Value { get; protected set; }
		public LiteralTree(Value val, Location loc)
		{
			this.Value = val;
			this.Location = loc;
		}
		public override string ToString()
		{
			return String.Format("Lit<{0}>:{1}", this.Value.Type, this.Value.ToString());
		}
		public override TExprRet Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			return itv.visitExpr(this);
		}
	}
}

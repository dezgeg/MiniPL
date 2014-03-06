using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.SemanticCheck;

namespace MiniLang.Frontend.Trees
{
	public abstract class StmtTree : Tree
	{
		public abstract void Accept<TExprRet>(ITreeVisitor<TExprRet> itv);
	}

	public class StmtListTree : StmtTree
	{
		public IList<StmtTree> Statements { get; protected set; }
		public StmtListTree()
		{
			this.Statements = new List<StmtTree>();
		}
		public override string ToString()
		{
			return "[" + String.Join(", ", Statements) + "]";
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmtList(this);
		}
	}
}

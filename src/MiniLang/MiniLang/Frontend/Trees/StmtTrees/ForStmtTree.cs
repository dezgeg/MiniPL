using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniLang.Frontend.Trees
{
	public class ForStmtTree : StmtTree
	{
		public VariableTree IteratorVar { get; protected set; }
		public ExprTree Min { get; protected set; }
		public ExprTree Max { get; protected set; }
		public StmtListTree Body { get; protected set; }
		public ForStmtTree(VariableTree iter, ExprTree min, ExprTree max, StmtListTree stmts)
		{
			this.IteratorVar = iter;
			this.Min = min;
			this.Max = max;
			this.Body = stmts;
		}
		public override string ToString()
		{
			return String.Format("For<{0}>({1}, {2}) {3}", IteratorVar, Min, Max, Body);
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmt(this);
		}

	}
}

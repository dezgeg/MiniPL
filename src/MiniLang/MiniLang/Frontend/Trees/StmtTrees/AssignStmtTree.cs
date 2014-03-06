using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend.Trees;

namespace MiniLang.Frontend.Trees
{
	public class AssignStmtTree : StmtTree
	{
		public VariableTree Variable { get; protected set; }
		public ExprTree Expr { get; protected set; }

		public AssignStmtTree(VariableTree variableTree, ExprTree exprTree)
		{
			this.Variable = variableTree;
			this.Expr = exprTree;
		}
		public override string ToString()
		{
			return String.Format("Assign({0}, {1})", Variable, Expr);
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmt(this);
		}
	}
}

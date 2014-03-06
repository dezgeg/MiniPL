using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniLang.Frontend.Trees
{
	public class PrintStmtTree : StmtTree
	{
		public ExprTree Expr { get; protected set; }

		public PrintStmtTree(ExprTree exprTree)
		{
			this.Expr = exprTree;
		}
		public override string ToString()
		{
			return "Print(" + Expr.ToString() + ")";
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmt(this);
		}

	}
	public class ReadStmtTree : StmtTree
	{
		public VariableTree Variable { get; protected set; }
		public ReadStmtTree(VariableTree var)
		{
			this.Variable = var;
		}
		public override string ToString()
		{
			return "Read(" + Variable.ToString() + ")";
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmt(this);
		}
	}
	public class AssertStmtTree : StmtTree
	{
		public ExprTree Expr { get; protected set; }

		public AssertStmtTree(ExprTree exprTree)
		{
			this.Expr = exprTree;
		}
		public override string ToString()
		{
			return "Assert(" + this.Expr.ToString() + ")";
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmt(this);
		}
	}
}

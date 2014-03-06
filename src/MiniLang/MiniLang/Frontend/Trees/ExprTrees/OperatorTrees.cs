using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend;

namespace MiniLang.Frontend.Trees
{
	public class BinopTree : ExprTree
	{
		public TokenType Operator { get; protected set; }
		public ExprTree Lhs { get; protected set; }
		public ExprTree Rhs { get; protected set; }
		public BinopTree(Token op, ExprTree lhs, ExprTree rhs)
		{
			this.Operator = op.Type;
			this.Location = op.Location;
			this.Lhs = lhs;
			this.Rhs = rhs;
		}
		public override string ToString()
		{
			return String.Format("Binop<{0}>({1}, {2})", this.Operator, this.Lhs, this.Rhs);
		}
		public override TExprRet Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			return itv.visitExpr(this);
		}
	}

	public class UnopTree : ExprTree
	{
		public TokenType Operator { get; protected set; }
		public ExprTree Operand { get; protected set; }

		public UnopTree(Token tok, ExprTree operand)
		{
			this.Operator = tok.Type;
			this.Operand = operand;
			this.Location = tok.Location;
		}
		public override string ToString()
		{
			return String.Format("Unop<{0}>({1})", this.Operator, this.Operand);
		}
		public override TExprRet Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			return itv.visitExpr(this);
		}
	}
}

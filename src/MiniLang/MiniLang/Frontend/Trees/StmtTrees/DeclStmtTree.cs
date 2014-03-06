using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniLang.Frontend.Trees
{

	public class DeclStmtTree : StmtTree
	{
		public VariableTree Var { get; protected set; }
		public TypeName Type { get; protected set; }
		public ExprTree Initializer { get; protected set; }

		public DeclStmtTree(VariableTree variable, TypeName type, ExprTree init)
		{
			this.Var = variable;
			this.Type = type;
			this.Initializer = init;
		}
		public override string ToString()
		{
			return String.Format("Decl<{0} {1}>({2})", this.Var, this.Type, this.Initializer);
		}
		public override void Accept<TExprRet>(ITreeVisitor<TExprRet> itv)
		{
			itv.visitStmt(this);
		}

	}
}

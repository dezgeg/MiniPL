using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend.Trees;
using MiniLang.Frontend;

namespace MiniLang.Frontend
{
	public interface ITreeVisitor<TExprRet>
	{
		TExprRet visitExpr(BinopTree tree);
		TExprRet visitExpr(LiteralTree tree);
		TExprRet visitExpr(UnopTree tree);
		TExprRet visitExpr(VariableTree tree);

		void visitStmt(AssignStmtTree tree);
		void visitStmt(AssertStmtTree tree);
		void visitStmt(DeclStmtTree tree);
		void visitStmt(ForStmtTree tree);
		void visitStmt(PrintStmtTree tree);
		void visitStmt(ReadStmtTree tree);

		void visitStmtList(StmtListTree tree);
	}
}

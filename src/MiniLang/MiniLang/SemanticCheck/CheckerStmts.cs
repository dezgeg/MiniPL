using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;

namespace MiniLang.SemanticCheck
{
	public partial class Checker
	{
		public void visitStmt(AssertStmtTree ast)
		{
			checkExprHasType(ast.Expr, TypeName.Bool, "Value in assert expression", ast.Expr.Location);
		}

		public void visitStmt(AssignStmtTree ast)
		{
			SymbolEntry sym = getSymbol(ast.Variable.Name, ast.Variable.Location);
			ast.Variable.SymbolEntry = sym;
			checkAssign(ast.Variable.Name, ast.Variable.Location);
			checkExprHasType(ast.Expr, sym.Type, "Value to be assigned to '" + ast.Variable.Name + "'", ast.Variable.Location);
		}
		public void visitStmt(DeclStmtTree dst)
		{
			VariableTree var = dst.Var;
			try
			{
				if (dst.Initializer != null)
					checkExprHasType(dst.Initializer, dst.Type, "Initializer for '" + var.Name + "'", dst.Initializer.Location);
			}
			catch (CompileException ce) // make sure that the variable is added to the symbol table even if the initializer is erroneous
			{
				this.Context.AddError(ce);
			}
			// declare afterwards, otherwise var i : int := i; would get through
			if (Context.SymbolTable.ContainsKey(var.Name))
			{
				SymbolEntry origSym = Context.SymbolTable[var.Name];
				throw new CompileException(String.Format("Redeclaration of variable '{0}'", var.Name), var.Location,
					String.Format("Variable '{0}' already declared type '{1}'", var.Name, origSym.Type), origSym.Location);
			}
			SymbolEntry sym = new SymbolEntry(dst.Type, var.Location);
			sym.Value = new Value(dst.Type);
			var.SymbolEntry = sym;
			Context.SymbolTable.Add(var.Name, sym);
		}
		public void visitStmt(ReadStmtTree rst)
		{
			SymbolEntry sym = getSymbol(rst.Variable.Name, rst.Variable.Location);
			rst.Variable.SymbolEntry = sym;
			checkAssign(rst.Variable.Name, rst.Variable.Location);
		}

		public void visitStmt(ForStmtTree fst)
		{
			try
			{
				checkExprHasType(fst.Min, TypeName.Int, "For-loop range start expression", fst.Min.Location);
			}
			catch (CompileException ce)
			{
				this.Context.AddError(ce);
			}
			try
			{
				checkExprHasType(fst.Max, TypeName.Int, "For-loop range end expression", fst.Max.Location);
			}
			catch (CompileException ce)
			{
				this.Context.AddError(ce);
			}

			SymbolEntry sym = null;
			try
			{
				sym = getSymbol(fst.IteratorVar.Name, fst.IteratorVar.Location);
				if (sym.Type != TypeName.Int)
					this.Context.AddError(new CompileException("For-loop iterator variable should be of type int", fst.IteratorVar.Location));
				fst.IteratorVar.SymbolEntry = sym;
			}
			catch (CompileException ce) // the iterator variable wasn't declared
			{
				this.Context.AddError(ce);
				// pretend that it is
				sym = new SymbolEntry(TypeName.Int, fst.IteratorVar.Location);
				this.Context.SymbolTable.Add(fst.IteratorVar.Name, sym);
			}
			sym.IsForLoopIterator = true;
			visitStmtList(fst.Body);
			sym.IsForLoopIterator = false;
		}
		public void visitStmt(PrintStmtTree pst)
		{
			pst.Expr.Accept(this);
		}
	}
}

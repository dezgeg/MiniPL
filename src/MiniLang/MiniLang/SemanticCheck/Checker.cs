using System;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;

namespace MiniLang.SemanticCheck
{
	public partial class Checker : ITreeVisitor<TypeName>
	{
		public CompilerContext Context { get; set; }
		private StmtListTree program;

		public Checker(StmtListTree root, CompilerContext ctx)
		{
			this.program = root;
			this.Context = ctx;
		}
		public Checker(StmtListTree root) : this(root, new CompilerContext(true)) { }
		public void Check()
		{
			program.Accept(this);
		}
		private Tuple<T1, T2> performBothErrorSafely<T1, T2>(Func<T1> f1, Func<T2> f2)
		{
			T1 res1;
			T2 res2;
			try
			{
				res1 = f1.Invoke();
			}
			catch (CompileException ce1)
			{
				try
				{
					res2 = f2.Invoke();
				}
				catch (CompileException) // both f1 and f2 caused an error
				{
					this.Context.AddError(ce1);
					throw;
				}
				throw; // just f1 caused an error
			}
			res2 = f2.Invoke();
			return new Tuple<T1, T2>(res1, res2);
		}
		public void checkAssign(String var, Location loc)
		{
			SymbolEntry sym = getSymbol(var, loc);
			if (sym.IsForLoopIterator)
				this.Context.AddError(new CompileException("Cannot assign to for loop iterator '" + var + "'",
					loc, "'" + var + "' declared", sym.Location));
		}

		private SymbolEntry getSymbol(string varName, Location loc)
		{
			if (!Context.SymbolTable.ContainsKey(varName))
				throw new CompileException("Undeclared variable '" + varName + "'", loc);
			return Context.SymbolTable[varName];
		}
		public void visitStmtList(StmtListTree list)
		{
			foreach (var stmt in list.Statements)
			{
				try
				{
					stmt.Accept(this);
				}
				catch (CompileException ce)
				{
					this.Context.AddError(ce);
				}
			}
		}
		public TypeName visitExpr(LiteralTree lit)
		{
			return lit.Value.Type;
		}
		public TypeName visitExpr(VariableTree var)
		{
			SymbolEntry sym = getSymbol(var.Name, var.Location);
			var.SymbolEntry = sym;
			return sym.Type;
		}
		public TypeName visitExpr(BinopTree op)
		{
			string opName = "operator '" + op.Operator + "'";

			switch (op.Operator)
			{
				case TokenType.And:
					return checkExprsHaveTypes(op.Lhs, op.Rhs, TypeName.Bool, opName, op.Location);
				case TokenType.Div:
				case TokenType.Minus:
				case TokenType.Plus:
				case TokenType.Times:
					return checkExprsHaveTypes(op.Lhs, op.Rhs, TypeName.Int, opName, op.Location);
				case TokenType.EQ:
				case TokenType.NE:
				case TokenType.LT:
				case TokenType.LE:
				case TokenType.GT:
				case TokenType.GE:
					checkSameTypes(op.Lhs, op.Rhs, op.Operator, op.Location);
					return TypeName.Bool;
			}
			throw new InvalidOperationException("BinopTree contained unexpected operand");
		}
		private TypeName checkSameTypes(ExprTree expr1, ExprTree expr2, TokenType oper, Location loc)
		{
			Tuple<TypeName, TypeName> types = performBothErrorSafely(() => expr1.Accept(this), () => expr2.Accept(this));
			if (types.Item1 != types.Item2)
				throw new CompileException(String.Format("Operands to operator '{0}' should have the same types, but are '{1}' and '{2}'",
					oper.ToString(), types.Item1, types.Item2), loc);
			return types.Item1;
		}
		private TypeName checkExprsHaveTypes(ExprTree expr1, ExprTree expr2, TypeName type, String oper, Location loc)
		{
			Tuple<TypeName, TypeName> types = performBothErrorSafely(() => expr1.Accept(this), () => expr2.Accept(this));
			if (types.Item1 != type || types.Item2 != type)
				throw new CompileException(String.Format("Both operands to {0} should have type '{1}', but are '{2}' and '{3}'",
					oper, type, types.Item1, types.Item2), loc);

			return type;
		}
		private TypeName checkExprHasType(ExprTree exprTree, TypeName expectedType, String msg, Location loc)
		{
			TypeName actualType = exprTree.Accept(this);
			if (actualType != expectedType)
				throw new CompileException(String.Format("{0} should have type '{1}', but has '{2}'",
					msg, expectedType, actualType), loc);
			return expectedType;
		}

		public TypeName visitExpr(UnopTree op)
		{
			if (op.Operator == TokenType.Not)
				return checkExprHasType(op.Operand, TypeName.Bool, "Operand to unary operator '!'", op.Location);
			else if (op.Operator == TokenType.Minus)
				return checkExprHasType(op.Operand, TypeName.Int, "Operand to unary operator '-'", op.Location);

			throw new InvalidOperationException("UnopTree contained unexpected operand");
		}
	}
}

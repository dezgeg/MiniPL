using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.SemanticCheck;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;

namespace MiniLang.Interpreter
{
	public class Interpreter : ITreeVisitor<Value>
	{
		public StmtListTree Program { get; set; }
		public Interpreter(StmtListTree program)
		{
			this.Program = program;
		}
		public void Interpret()
		{
			foreach (StmtTree tree in Program.Statements)
				tree.Accept(this);
		}
		// XXX: why does overriding these require this 'ITreeVisitor<Foo>.' prefix?
		Value ITreeVisitor<Value>.visitExpr(BinopTree tree)
		{
			Value lhs = tree.Lhs.Accept(this), rhs = tree.Rhs.Accept(this);
			switch (tree.Operator)
			{
				case TokenType.And: return new Value(lhs.BoolValue && rhs.BoolValue);

				case TokenType.Div:
					if (rhs.IntValue == 0)
						throw new CompileException("Division by zero", tree.Location);
					return new Value(lhs.IntValue / rhs.IntValue);
				case TokenType.Minus: return new Value(lhs.IntValue - rhs.IntValue);
				case TokenType.Plus: return new Value(lhs.IntValue + rhs.IntValue);
				case TokenType.Times: return new Value(lhs.IntValue * rhs.IntValue);

				case TokenType.EQ: return new Value(lhs.CompareTo(rhs) == 0);
				case TokenType.NE: return new Value(lhs.CompareTo(rhs) != 0);
				case TokenType.LT: return new Value(lhs.CompareTo(rhs) < 0);
				case TokenType.LE: return new Value(lhs.CompareTo(rhs) <= 0);
				case TokenType.GT: return new Value(lhs.CompareTo(rhs) > 0);
				case TokenType.GE: return new Value(lhs.CompareTo(rhs) >= 0);
			}
			throw new SystemException();
		}

		Value ITreeVisitor<Value>.visitExpr(LiteralTree tree)
		{
			return tree.Value;
		}

		Value ITreeVisitor<Value>.visitExpr(UnopTree tree)
		{
			Value operandValue = tree.Operand.Accept(this);
			switch (tree.Operator)
			{
				case TokenType.Minus: return new Value(-operandValue.IntValue);
				case TokenType.Not: return new Value(!operandValue.BoolValue);
			}
			throw new SystemException();
		}

		Value ITreeVisitor<Value>.visitExpr(VariableTree tree)
		{
			return tree.SymbolEntry.Value;
		}

		void ITreeVisitor<Value>.visitStmt(AssignStmtTree tree)
		{
			tree.Variable.SymbolEntry.Value = tree.Expr.Accept(this);
		}

		void ITreeVisitor<Value>.visitStmt(AssertStmtTree tree)
		{
			if (!tree.Expr.Accept(this).BoolValue)
				throw new CompileException("Assertion failed", tree.Expr.Location);
		}

		void ITreeVisitor<Value>.visitStmt(DeclStmtTree tree)
		{
			if(tree.Initializer != null) // Default values will always be assigned in Checker
				tree.Var.SymbolEntry.Value = tree.Initializer.Accept(this);
		}

		void ITreeVisitor<Value>.visitStmt(ForStmtTree tree)
		{
			SymbolEntry sym = tree.IteratorVar.SymbolEntry;
			int start = tree.Min.Accept(this).IntValue, end = tree.Max.Accept(this).IntValue;
			for (int i = start; i <= end; i++)
			{
				sym.Value = new Value(i);
				tree.Body.Accept(this);
			}
			sym.Value = new Value(end + 1); // Sample program 2 seems to assume this.
		}
		void ITreeVisitor<Value>.visitStmt(PrintStmtTree tree)
		{
			Value value = tree.Expr.Accept(this);
			Console.Write(value.ToString().Replace("\n", "\r\n")); // bit hacky, but otherwise the test files look stupid in notepad
		}
		void ITreeVisitor<Value>.visitStmt(ReadStmtTree tree)
		{
			SymbolEntry sym = tree.Variable.SymbolEntry;
			String line = Console.ReadLine();
			if (line == null)
				throw new CompileException("No input available for 'read' statement", tree.Variable.Location);
			sym.Value = Value.FromString(sym.Type, line, tree.Variable.Location);
		}
		public void visitStmtList(StmtListTree tree)
		{
			foreach (var stmt in tree.Statements)
				stmt.Accept(this);
		}
	}
}

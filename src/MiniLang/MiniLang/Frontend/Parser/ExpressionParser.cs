using System;
using MiniLang.Frontend.Trees;

namespace MiniLang.Frontend
{
	public partial class Parser
	{
		ExprTree parseLogical()
		{
			ExprTree lhs = parseCmp();
			Token tok = null;
			while (matchTokens(ref tok, TokenType.And))
			{
				ExprTree rhs = parseCmp();
				lhs = new BinopTree(tok, lhs, rhs);
			}
			return lhs;
		}

		ExprTree parseCmp()
		{
			ExprTree lhs = parseTerm();
			Token tok = null;
			if (matchTokens(ref tok, TokenType.LT, TokenType.LE, TokenType.GT, TokenType.GE, TokenType.EQ, TokenType.NE))
			{
				ExprTree rhs = parseTerm();
				lhs = new BinopTree(tok, lhs, rhs);
			}
			if (matchTokens(ref tok, TokenType.LT, TokenType.LE, TokenType.GT, TokenType.GE, TokenType.EQ, TokenType.NE))
				throw new CompileException("Nonassociative operator '" + tok.GetContent() + "' used in associative context", tok.Location);
			return lhs;
		}

		ExprTree parseTerm()
		{
			ExprTree lhs = parseFactor();
			Token tok = null;
			while (matchTokens(ref tok, TokenType.Plus, TokenType.Minus))
			{
				ExprTree rhs = parseFactor();
				lhs = new BinopTree(tok, lhs, rhs);
			}
			return lhs;
		}
		ExprTree parseFactor()
		{
			ExprTree lhs = parseUnary();
			Token tok = null;
			while (matchTokens(ref tok, TokenType.Times, TokenType.Div))
			{
				ExprTree rhs = parseUnary();
				lhs = new BinopTree(tok, lhs, rhs);
			}
			return lhs;
		}
		private ExprTree parseUnary()
		{
			Token tok = null;
			if (matchTokens(ref tok, TokenType.Minus, TokenType.Not))
				return new UnopTree(tok, parseUnary());
			return parseParenExpr();
		}
		private ExprTree parseParenExpr()
		{
			Token tok = expectTokens(TokenType.LeftParen, TokenType.Identifier, TokenType.IntLiteral, TokenType.False, TokenType.True, TokenType.StringLiteral);
			if (tok.Type == TokenType.LeftParen)
			{
				ExprTree retval = ParseExpr();
				expectTokens(TokenType.RightParen);
				return retval;
			}
			else if (tok.Type == TokenType.Identifier)
				return new VariableTree(tok);
			else if (tok.Type == TokenType.IntLiteral || tok.Type == TokenType.False || tok.Type == TokenType.True || tok.Type == TokenType.StringLiteral)
				return new LiteralTree(valueFromToken(tok), tok.Location);
			else throw new InvalidOperationException();
		}

	}
}

using System;
using System.Linq;
using MiniLang.Frontend.Trees;

namespace MiniLang.Frontend
{
	public partial class Parser
	{
		// predict set for statement
		private static readonly TokenType[] STMT_PREDICT = { TokenType.Var, TokenType.For, TokenType.Read, TokenType.Print, TokenType.Assert, TokenType.Identifier };
		// synchronization set for statement
		private static readonly TokenType[] STMT_SYNCH_SET = { TokenType.Var, TokenType.For, TokenType.Read, TokenType.Print, TokenType.Assert, TokenType.End, TokenType.EndOfFile, TokenType.Semicolon };
		// synchronization set for a for-statement
		private static readonly TokenType[] FOR_SYNCH_SET = { TokenType.Do, TokenType.Var, TokenType.For, TokenType.Read, TokenType.Print, TokenType.Assert, TokenType.End, TokenType.EndOfFile, TokenType.Semicolon };
		// statement keyword start tokens, used for error recovery
		private static readonly TokenType[] STMT_KW_PREDICT = { TokenType.Var, TokenType.For, TokenType.Read, TokenType.Print, TokenType.Assert, TokenType.EndOfFile };

		public CompilerContext Context { get; set; }
		private Lexer lexer;
		public Parser(Lexer lex, CompilerContext ctx)
		{
			this.lexer = lex;
			this.Context = ctx;
		}
		public Parser(Lexer lex) : this(lex, new CompilerContext(true)) { }

		public ExprTree ParseExpr()
		{
			return parseLogical();
		}

		private Token synchronize(TokenType[] synchSet)
		{
			while (!synchSet.Contains(this.lexer.PeekToken().Type))
				this.lexer.NextToken();
			return this.lexer.PeekToken();
		}

		private Token expectTokens(params TokenType[] types)
		{
			Token tok = null;
			if (matchTokens(ref tok, types))
				return tok;
			else
			{
				tok = lexer.PeekToken();
				throw new CompileException(String.Format("Unexpected token '{0}' near '{1}', expecting one of {2}", tok.Type, tok.GetContent().Replace('\n', ' '), String.Join(", ", types)), tok.Location);
			}
		}

		private bool matchTokens(ref Token token, params TokenType[] types)
		{
			Token current = lexer.PeekToken();
			foreach (var type in types)
				if (current.Type == type)
				{
					token = lexer.NextToken();
					return true;
				}
			return false;
		}

		public StmtListTree ParseProgram()
		{
			StmtListTree stmts = parseStatements();
			try
			{
				expectTokens(TokenType.EndOfFile);
			}
			catch (CompileException ce)
			{
				this.Context.AddError(ce);
			}
			return stmts;
		}
		private VariableTree parseIdent()
		{
			return new VariableTree(expectTokens(TokenType.Identifier));
		}

		private bool predict(TokenType[] predictSet)
		{
			return predictSet.Any(t => t == lexer.PeekToken().Type);
		}
		private Value valueFromToken(Token tok)
		{
			TypeName resultType =
				tok.Type == TokenType.IntLiteral ? TypeName.Int :
				tok.Type == TokenType.StringLiteral ? TypeName.String :
				TypeName.Bool;
			return Value.FromString(resultType, tok.GetContent(), tok.Location);
		}
	}
}

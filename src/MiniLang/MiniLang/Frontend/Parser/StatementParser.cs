using System;
using System.Linq;
using MiniLang.Frontend.Trees;

namespace MiniLang.Frontend
{
	public partial class Parser
	{
		private StmtListTree parseStatements()
		{
			StmtListTree stmtList = new StmtListTree();

			do
			{
				StmtTree stmt;
				try
				{
					stmt = parseStmt();
				}
				catch (CompileException ce) // Error in statement: skip to next ';', 'end' or statement start symbol
				{
					this.Context.AddError(ce);
					Token lookahead = this.synchronize(STMT_SYNCH_SET);
					if (lookahead.Type == TokenType.Semicolon)
						lexer.NextToken();
					continue;
				}

				try
				{
					expectTokens(TokenType.Semicolon);
					stmtList.Statements.Add(stmt);
				}
				/* Two cases:
				 * - print foo + bar print baz; i.e just a missing semicolon, and the next line starts with a stmt keyword:
				 *     Assume that the first statement was ok and pretend the semicolon was there.
				 * - x := a 0 b + c; print x; i.e an error in the middle of an expression
				 *     Here the assignment has been parsed as 'x := a', which is obviously incomplete, and must not be passed on to semantic check.
				 *     So, throw away the statement, and resynchronize
				 */
				catch (CompileException ce)
				{
					this.Context.AddError(ce);
					if (STMT_KW_PREDICT.Contains(this.lexer.PeekToken().Type))
					{
						stmtList.Statements.Add(stmt);
						continue;
					}
					Token lookahead2 = this.synchronize(STMT_SYNCH_SET);
					if (lookahead2.Type == TokenType.Semicolon)
						lexer.NextToken();
				}
			} while (predict(STMT_PREDICT));

			return stmtList;
		}

		private StmtTree parseStmt()
		{
			Token tok = null;

			if (matchTokens(ref tok, TokenType.For))
			{
				VariableTree variable = null;
				ExprTree min = null, max = null;
				// Make sure that we don't get 'unexpected end for' errors if there's an error in the for declaration.
				try
				{
					variable = parseIdent();
					expectTokens(TokenType.In);
					min = ParseExpr();
					expectTokens(TokenType.DotDot);
					max = ParseExpr();
					expectTokens(TokenType.Do);
				}
				catch (CompileException ce)
				{
					Context.AddError(ce);
					this.synchronize(FOR_SYNCH_SET);
					if (this.lexer.PeekToken().Type == TokenType.Do)
						this.lexer.NextToken();
					if (this.lexer.PeekToken().Type == TokenType.Semicolon)
						this.lexer.NextToken();
					// Create phony expressions for the start & end so that Checker does not bite nulls
					min = min ?? new LiteralTree(new Value(0), tok.Location);
					max = max ?? new LiteralTree(new Value(0), tok.Location);
				}

				StmtListTree stmts = parseStatements();

				expectTokens(TokenType.End);
				expectTokens(TokenType.For);
				return new ForStmtTree(variable, min, max, stmts);
			}
			else if (matchTokens(ref tok, TokenType.Var))
			{
				VariableTree variable = parseIdent();
				expectTokens(TokenType.Colon);
				TypeName type = parseTypeName();
				ExprTree init = null;

				Token assignTok = null;
				if (matchTokens(ref assignTok, TokenType.Assign))
					init = ParseExpr();
				return new DeclStmtTree(variable, type, init);
			}
			else if (matchTokens(ref tok, TokenType.Identifier))
			{
				expectTokens(TokenType.Assign);
				return new AssignStmtTree(new VariableTree(tok), ParseExpr());
			}
			else if (matchTokens(ref tok, TokenType.Read))
				return new ReadStmtTree(parseIdent());
			else if (matchTokens(ref tok, TokenType.Print))
				return new PrintStmtTree(ParseExpr());
			else if (matchTokens(ref tok, TokenType.Assert))
				return new AssertStmtTree(ParseExpr());
			else { expectTokens(STMT_PREDICT); return null;  } 
		}

		private TypeName parseTypeName()
		{
			return (TypeName)expectTokens(TokenType.Int, TokenType.Bool, TokenType.String).Type;
		}
	}
}

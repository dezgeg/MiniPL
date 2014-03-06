using MiniLang.Frontend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MiniLangTests
{
	[TestClass()]
	public class LexerTest
	{
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerConstructorTest()
		{
			Lexer tokenizer = new Lexer("test.mpl", "foo");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(tok.Type, TokenType.Identifier);
			Assert.AreEqual(tok.GetContent(), "foo");
			Assert.AreEqual(tok.Location, new Location(1, 0, 0, 3));
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerConstructorTestTwoTokens()
		{
			Lexer tokenizer = new Lexer("test.mpl", " a bcd");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(tok.Type, TokenType.Identifier);
			Assert.AreEqual(tok.GetContent(), "a");
			Assert.AreEqual(tok.Location, new Location(1, 1, 1, 1));

			tok = tokenizer.NextToken();
			Assert.AreEqual(tok.Type, TokenType.Identifier);
			Assert.AreEqual(tok.GetContent(), "bcd");
			Assert.AreEqual(tok.Location, new Location(1, 3, 3, 3));
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestComment()
		{
			Lexer tokenizer = new Lexer("test.mpl", "/* asdasd */ test");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.Identifier, new Location(1, 13, 13, 4), "test"), tok);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestNestedComment()
		{
			Lexer tokenizer = new Lexer("test.mpl", "/* asd /* asdasd */ asd */ test");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.Identifier, new Location(1, 27, 27, 4), "test"), tok);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestEolComment()
		{
			Lexer tokenizer = new Lexer("test.mpl", "// asdasd \r\ntest// barbar");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.Identifier, new Location(2, 0, 12, 4), "test"), tok);
			tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.EndOfFile, new Location(3, 0, 0, 0), ""), tok);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestNewline()
		{
			Lexer tokenizer = new Lexer("test.mpl", "     \n1234");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.IntLiteral, new Location(2, 0, 6, 4), "1234"), tok);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestStringLiteral()
		{
			Lexer tokenizer = new Lexer("test.mpl", "\"abcde\"");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.StringLiteral, new Location(1, 0, 0, 7), "abcde"), tok);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestStringLiteralEscapes()
		{
			Lexer tokenizer = new Lexer("test.mpl", "\"ab\\\\d\\\"e\"");
			Token tok = tokenizer.NextToken();
			Assert.AreEqual(new Token(TokenType.StringLiteral, new Location(1, 0, 0, 10), "ab\\d\"e"), tok);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestOperators()
		{
			TokenType[] types = { TokenType.And, TokenType.Assign, TokenType.Colon, TokenType.Div,
									TokenType.EQ, TokenType.GE, TokenType.GT, TokenType.LE,
									TokenType.LeftParen, TokenType.LT, TokenType.Minus,
									TokenType.NE, TokenType.Not, TokenType.Plus,
									TokenType.DotDot, TokenType.RightParen,
									TokenType.Semicolon, TokenType.Times };
			Lexer tokenizer = new Lexer("test.mpl", "& := : / = >= > <= ( < - != ! + .. ) ; *");
			List<TokenType> actuals = new List<TokenType>();
			foreach(TokenType type in types)
			{
				Token actual;
				try { actual = tokenizer.NextToken();
				} catch (Exception) { break; }
				actuals.Add(actual.Type);
			}
			Assert.AreEqual(String.Join(", ", types), String.Join(", ", actuals));
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestKeywords()
		{
			TokenType[] types = { TokenType.Var, TokenType.For, TokenType.End, TokenType.In, TokenType.Do, TokenType.Bool,
									TokenType.Read, TokenType.Print, TokenType.Int, TokenType.String, TokenType.Assert };
			Lexer tokenizer = new Lexer("test.mpl", "var for end in do bool read print int string assert");
			List<TokenType> actuals = new List<TokenType>();
			foreach (TokenType type in types)
			{
				Token actual;
				try { actual = tokenizer.NextToken();
				} catch (Exception) { break; }
				actuals.Add(actual.Type);
			}
			Assert.AreEqual(String.Join(", ", types), String.Join(", ", actuals));
		}

	}
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniLang.Frontend;
using MiniLang;

namespace MiniLangTests
{
	[TestClass]
	public class ParseErrorTests
	{
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestUnexpectedAsciiChars()
		{
			Lexer lex = new Lexer("test", "`'");
			Token t1 = lex.NextToken(), t2 = lex.NextToken();

			Assert.AreEqual(new Token(TokenType.Error, new Location(1, 0, 0, 1), "`"), t1);
			Assert.AreEqual(new Token(TokenType.Error, new Location(1, 1, 1, 1), "'"), t2);
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestUnexpectedNonAsciiChars()
		{
			Lexer lex = new Lexer("test", "\U0001F4A9"); // Unicode Character 'PILE OF POO' http://www.fileformat.info/info/unicode/char/1f4a9/index.htm
			try { lex.NextToken(); Assert.Fail(); }
			catch (CompileException) { }
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void TokenizerTestUnterminatedComment()
		{
			Lexer lex = new Lexer("test", "/*");
			try { lex.NextToken(); Assert.Fail(); }
			catch (CompileException) { }
		}
	}
}

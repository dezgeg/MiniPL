using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;
using MiniLang;

namespace MiniLangTests
{
	[TestClass()]
	public class ParserExprTest
	{
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestComplexExpression()
		{
			Parser p = new Parser(new Lexer("test.mpl", "1 * 2 + 3"));
			Tree t = p.ParseExpr();
			Assert.AreEqual("Binop<Plus>(Binop<Times>(Lit<Int>:1, Lit<Int>:2), Lit<Int>:3)", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestParenExpression()
		{
			Parser p = new Parser(new Lexer("test.mpl", "(1 + 2) * 3"));
			Tree t = p.ParseExpr();
			Assert.AreEqual("Binop<Times>(Binop<Plus>(Lit<Int>:1, Lit<Int>:2), Lit<Int>:3)", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestAssociativity()
		{
			Parser p = new Parser(new Lexer("test.mpl", "3 - 2 - 1"));
			Tree t = p.ParseExpr();
			Assert.AreEqual("Binop<Minus>(Binop<Minus>(Lit<Int>:3, Lit<Int>:2), Lit<Int>:1)", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestUnaryExpression()
		{
			Parser p = new Parser(new Lexer("test.mpl", "a + -bc"));
			Tree t = p.ParseExpr();
			Assert.AreEqual("Binop<Plus>(Var:a, Unop<Minus>(Var:bc))", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestMultipleBinops()
		{
			Parser p = new Parser(new Lexer("test.mpl", "1 + 2 * -3 / 4"));
			Tree t = p.ParseExpr();
			Assert.AreEqual("Binop<Plus>(Lit<Int>:1, Binop<Div>(Binop<Times>(Lit<Int>:2, Unop<Minus>(Lit<Int>:3)), Lit<Int>:4))", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestNonassocOperators()
		{
			Parser p = new Parser(new Lexer("test.mpl", "print 1 < 2 < 3;"));
			Tree t = null;
			try
			{
				t = p.ParseProgram();
			}
			catch (CompileException)
			{
			}
			Assert.IsNull(t);
		}
	}
}

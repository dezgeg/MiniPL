using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;

namespace MiniLangTests
{
	[TestClass()]
	public class ParserStmtTests
	{
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestAssign()
		{
			Parser p = new Parser(new Lexer("test.mpl", "foo := 42;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Assign(Var:foo, Lit<Int>:42)]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestDeclaration()
		{
			Parser p = new Parser(new Lexer("test.mpl", "var x : bool;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Decl<Var:x Bool>()]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestDeclarationWithInitializer()
		{
			Parser p = new Parser(new Lexer("test.mpl", "var x : int := 2;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Decl<Var:x Int>(Lit<Int>:2)]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestAssert()
		{
			Parser p = new Parser(new Lexer("test.mpl", "assert x < y;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Assert(Binop<LT>(Var:x, Var:y))]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestRead()
		{
			Parser p = new Parser(new Lexer("test.mpl", "read x;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Read(Var:x)]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestPrint()
		{
			Parser p = new Parser(new Lexer("test.mpl", "print xyz & foo;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Print(Binop<And>(Var:xyz, Var:foo))]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestFor()
		{
			Parser p = new Parser(new Lexer("test.mpl", "for z in 1..10\ndo\n\tprint z;\nend for;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[For<Var:z>(Lit<Int>:1, Lit<Int>:10) [Print(Var:z)]]", t.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void ParserTestMultipleStatements()
		{
			Parser p = new Parser(new Lexer("test.mpl", "foo := 41; print foo;"));
			Tree t = p.ParseProgram();
			Assert.AreEqual("[Assign(Var:foo, Lit<Int>:41), Print(Var:foo)]", t.ToString());
		}
	}
}

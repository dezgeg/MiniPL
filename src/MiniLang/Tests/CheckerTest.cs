using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;
using MiniLang.SemanticCheck;
using MiniLang;

namespace MiniLangTests
{
	[TestClass()]
	public class CheckerTest
	{
		private static void checkOk(string program)
		{
			Parser p = new Parser(new Lexer("test.mpl", program));
			Checker c = new Checker(p.ParseProgram());
			c.Check();
		}
		private static void checkExprOk(string program)
		{
			Parser p = new Parser(new Lexer("test.mpl", "print " + program + ";"));
			Checker c = new Checker(p.ParseProgram());
			c.Check();
		}
		private void checkExprFail(string prog)
		{
			try
			{
				checkExprOk(prog);
				Assert.Fail();
			}
			catch (CompileException) { }
		}
		private void checkFail(string prog)
		{
			try
			{
				checkOk(prog);
				Assert.Fail();
			}
			catch (CompileException) { }
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckAssertOk()
		{
			checkOk("assert 0 < 1;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckAssertFail()
		{
			checkFail("assert 1 + 2;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckArithmeticTypeMismatch()
		{
			checkExprOk("1 + 2 * -3 / 4");
			checkExprFail("1 + false");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckLogicTypeMismatch()
		{
			checkExprOk("false & true");
			checkExprOk("!false");
			checkExprOk("\"foo\" = \"bar\"");
			checkExprFail("1 + \"bar\"");
			checkExprFail("1 = true");
			checkExprFail("!\"bar\"");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckUndeclaredVariable()
		{
			checkFail("print a;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckPrint()
		{
			checkOk("print \"foo\";");
			checkOk("print 123;");
			checkOk("print true;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckAlreadyDeclaredVariable()
		{
			checkFail("var a : int; var a : int;");
			checkFail("var a : int; var a : string;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckDeclInitializer()
		{
			checkOk("var a : int := 1 + 2;");
			checkFail("var a : int := 1 <= 2;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckAssignment()
		{
			checkOk("var a : int; a := 2;");
			checkFail("var s : string; s := 3;");
			checkFail("var i : int := i;");
			checkFail("a := 5;");
			checkFail("for j in 1..10 do j := 2; end for;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckFor()
		{
			checkOk("var i : int; for i in 1 + 10..10 * 3 do print i + 1; end for;");
			checkFail("var j : int; for j in \"asd\"..10 do print j; end for;");
			checkFail("var k : int; for jk in 10..false do print k; end for;");
			checkFail("for undeclared in 1..2 do print undeclared; end for;");
			checkFail("var s : string; for s in 1..10 do print s; end for;");
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void CheckerCheckRead()
		{
			checkOk("var x : int; read x;");
			checkOk("var x : string; read x;");
			checkOk("var x : bool; read x;");
			checkFail("read x;");
			checkFail("for x in 1..10 do read x; end for;");
		}
	}
}

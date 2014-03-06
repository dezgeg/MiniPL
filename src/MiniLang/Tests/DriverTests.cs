using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MiniLang.Frontend;
using MiniLang.Frontend.Trees;
using MiniLang.SemanticCheck;
using MiniLang;
using System.IO;
using System.Reflection;
using System.Text;

namespace MiniLangTests
{
	[TestClass()]
	public class DriverTest
	{
		public static String ReadFile(string path, string basename, string ext)
		{
			try
			{
				return File.ReadAllText(path + "\\" + basename + "." + ext);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void DoCmdlineCodeTest()
		{
			StringWriter stdout = new StringWriter();
			Console.SetOut(stdout);

			int status = Program.Instance.RealMain(new String[] { "-e", "print 42;" });
			Assert.AreEqual(0, status);
			Assert.AreEqual("42", stdout.ToString());
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void DoCmdlineNonexistingFileTest()
		{
			StringWriter stderr = new StringWriter();
			Console.SetError(stderr);

			int status = Program.Instance.RealMain(new String[] { "asdasadasdasadasadasadasadsadasadsadasdasa" });
			Assert.AreEqual(4, status);
			Assert.IsTrue(stderr.ToString().Contains("Could not find file"));
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void DoCmdlineShowsUsageTest()
		{
			StringWriter stderr = new StringWriter();
			Console.SetError(stderr);

			int status = Program.Instance.RealMain(new String[] { });
			Assert.AreEqual(3, status);
			Assert.IsTrue(stderr.ToString().Contains("usage:"));
		}
		[TestMethod()]
		[DeploymentItem("MiniLang.exe")]
		public void DoDriverTests()
		{
			bool errors = false;
			string path = @"..\..\..\Tests\DriverTests";
			DirectoryInfo di = new DirectoryInfo(path);
			foreach (FileInfo fi in di.GetFiles("*.mpl"))
			{
				string basename = Path.GetFileNameWithoutExtension(fi.Name);
				string stdin = ReadFile(path, basename, "in") ?? "";
				string expectedStdout = ReadFile(path, basename, "out");
				string expectedStderr = ReadFile(path, basename, "err");
				int expectedStatus;
				if (expectedStderr == null)
					expectedStatus = 0;
				else
					expectedStatus = expectedStdout == null ? 2 : 3;
				expectedStdout = expectedStdout ?? "";
				expectedStderr = expectedStderr ?? "";

				StringWriter stdout = new StringWriter();
				StringWriter stderr = new StringWriter();
				Console.SetIn(new StringReader(stdin));
				Console.SetOut(stdout);
				Console.SetError(stderr);
				int status = Program.Instance.RealMain(new String[] { path + "\\" + fi.Name });
				if(expectedStderr != stderr.ToString())
				{
					errors = true;
					File.WriteAllText(path + "\\" + basename + ".err.ACTUAL", stderr.ToString());
				}
				if(expectedStdout != stdout.ToString())
				{
					errors = true;
					File.WriteAllText(path + "\\" + basename + ".out.ACTUAL", stdout.ToString());
				}
			}
			Assert.IsFalse(errors);
		}
	}
}

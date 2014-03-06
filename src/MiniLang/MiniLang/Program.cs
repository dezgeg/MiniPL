using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.Frontend;
using System.IO;
using MiniLang.SemanticCheck;

using MiniLang.Interpreter;
using MiniLang.Frontend.Trees;

namespace MiniLang
{
	public class Program
	{
		// Added as fields so they get into the class diagram
		private CompilerContext Context;
		private Lexer Lexer;
		private Parser Parser;
		private Checker Checker;
		private StmtListTree CompiledProgram;
		private MiniLang.Interpreter.Interpreter Interpreter;
		public static Program Instance = new Program();
		public static void Main(String[] args)
		{
			Environment.ExitCode = Instance.RealMain(args);
		}
		// Entry point used by driver tests
		// Returns an exit code passed to the operating system
		// 0 - ok, 1 - compile error, 2 - runtime error, 3 - invalid command line parameters, 4 - error opening source file, 5 - fatal internal error
		public int RealMain(String[] args)
		{
			Context = new CompilerContext(false);
			try
			{
				if (args.Length == 2 && args[0] == "-e")
					this.Lexer = new Lexer("<command line>", args[1]);
				else if (args.Length == 1)
				{
					try
					{
						this.Lexer = new Lexer(args[0]);
					}
					catch (IOException ioe)
					{
						Console.Error.WriteLine(ioe.Message);
						return 4;
					}
				}
				else
				{
					Console.Error.WriteLine("usage: 'MiniLang <filename>' or 'MiniLang -e <code>'");
					return 3;
				}
				this.Parser = new Parser(this.Lexer, this.Context);
				this.CompiledProgram = Parser.ParseProgram();
				this.Checker = new Checker(this.CompiledProgram, this.Context);
				this.Checker.Check();
				if(this.Context.Errors.Count == 0)
					try
					{
						this.Interpreter = new MiniLang.Interpreter.Interpreter(this.CompiledProgram);
						this.Interpreter.Interpret();
					}
					catch (CompileException ce)
					{
						Console.Error.WriteLine("Runtime error:");
						Console.Error.WriteLine(ce.GetFriendlyMessage());
						return 2;
					}
			}
			catch (CompileException ce)
			{
				this.Context.AddError(ce);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("---BEGIN FATAL INTERNAL ERROR---");
				Console.Error.WriteLine(e);
				Console.Error.WriteLine("---END FATAL INTERNAL ERROR---");
				return 5;
			}
			this.Context.Errors.Sort();
			foreach (var ce in this.Context.Errors)
				Console.Error.WriteLine(ce.GetFriendlyMessage());
			if (this.Context.Errors.Count != 0)
			{
				Console.Error.WriteLine("\r\n" + this.Context.Errors.Count + " errors in total.");
				return 1;
			}
			return 0;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang.SemanticCheck;

namespace MiniLang.Frontend
{
	public abstract class ExprTree : Tree
	{
		public abstract TExprRet Accept<TExprRet>(ITreeVisitor<TExprRet> itv);
		public Location Location { get; protected set; }
	}

}

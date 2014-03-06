using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniLang;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace MiniLang.Frontend
{
	public partial class Lexer
	{
		private static readonly IDictionary<String, TokenType> keywords = new Dictionary<String, TokenType>();
		static Lexer()
		{
			TokenType[] types = { TokenType.Var, TokenType.For, TokenType.End, TokenType.In, TokenType.Do, TokenType.Bool,
									TokenType.Read, TokenType.Print, TokenType.Int, TokenType.String, TokenType.Assert, TokenType.False, TokenType.True };
			foreach (var type in types)
				keywords.Add(type.ToString().ToLower(), type);
		}
		private int fileOffset;
		private String fileContent;
		private Token currentToken;

		public String FileName { get; private set; }
		public Location CurrentLocation { get; private set; }

		public Lexer(String fileName)
			: this(fileName, File.ReadAllText(fileName))
		{
		}
		public Lexer(String fileName, String content)
		{
			this.fileContent = content;
			this.FileName = fileName;
			this.CurrentLocation = new Location();
		}

		private void updateCurrentToken()
		{
			Token tok;
			do
			{
				tok = dfaParseToken(MainParserDfaTable);
				if (tok.Type == TokenType.CommentStart)
					skipComments(tok.Location);
			} while (tok.Type == TokenType.Newline || tok.Type == TokenType.Whitespace || tok.Type == TokenType.CommentStart);

			string content;
			// Keyword screening
			if (tok.Type == TokenType.Identifier && keywords.ContainsKey(content = tok.GetContent()))
					tok.Type = keywords[content];
			else if (tok.Type == TokenType.StringLiteral)
			{
				// Fix up string literals as soon as possible: remove surrounding quotes and translate escape characters
				StringBuilder sb = new StringBuilder();
				int endOffset = tok.Location.Offset + tok.Location.Length;
				// +1 and -1 ignore the surrounding quotes
				for (int i = tok.Location.Offset + 1; i < endOffset - 1; i++)
				{
					if (this.fileContent[i] == '\\')
					{
						i++; // just skip escaping \ for \" and \\, the DFA parser has ensured that no other cases will get here.
						if (this.fileContent[i] == 'n')
						{
							sb.Append("\n");
							continue;
						}
					}
					sb.Append(this.fileContent[i]);
				}
				tok.SetContent(sb.ToString());
			}

			this.currentToken = tok;
		}
		public Token NextToken()
		{
			Token retval = PeekToken();
			this.currentToken = null;
			return retval;
		}
		public Token PeekToken()
		{
			if (this.currentToken == null)
				this.updateCurrentToken();
			return this.currentToken;
		}

		private void skipComments(Location startLoc)
		{
			int nesting = 1;
			while (nesting > 0)
			{
				Token tok = dfaParseToken(CommentParserDfaTable);
				switch (tok.Type)
				{
					case TokenType.CommentStart:
						nesting++;
						break;
					case TokenType.CommentEnd:
						nesting--;
						break;
					case TokenType.Newline:
						break;
					case TokenType.EndOfFile:
						throw new CompileException("Unterminated comment", tok.Location, "Comment begun", startLoc);
					default:
						throw new InvalidDataException("Comment parser returned an unexpected token");
				}
			}
		}

		private Token dfaParseToken(byte[][] dfa)
		{
			int startOffset = this.fileOffset;
			int curOffset = startOffset;
			byte state = 1;

			while (true)
			{
				char c;
				try
				{
					c = this.fileContent[curOffset];
					if (c == '\0')
						throw new CompileException("Illegal NUL in source code", CurrentLocation);
				}
				catch (IndexOutOfRangeException)
				{
					// End of input occured, but since we have one-character lookahead, there still may be a partial token to be returned
					if (state == 1) // if in initial state, return EOF directly
						return new Token(TokenType.EndOfFile, this.CurrentLocation, "");
					else
						c = '\0';
				}
				byte nextState;
				try
				{
					nextState = dfa[state][(int)c];
				}
				catch (IndexOutOfRangeException)
				{
					throw new CompileException("Non-ASCII character in source code", CurrentLocation);
				}

				if (nextState < 128)
				{
					state = nextState;
					curOffset++;
				}
				else
				{
					short tokenLength = (short)(curOffset - startOffset);
					Location tokenLoc = (Location)this.CurrentLocation.Clone();
					tokenLoc.Length = tokenLength;
					tokenLoc.Offset = startOffset;

					TokenType type = (TokenType)(int)nextState;

					if (type == TokenType.Newline)
					{
						this.CurrentLocation.Line += 1;
						this.CurrentLocation.Column = 0;
					}
					else
						this.CurrentLocation.Column += tokenLength;
					this.fileOffset = curOffset;
					if(startOffset + tokenLength <= fileContent.Length)
						return new Token(type, tokenLoc, this.fileContent.Substring(startOffset, tokenLength));
					else
						return new Token(type, tokenLoc, this.fileContent.Substring(startOffset, tokenLength - 1));
				}
			}
		}
	}

}

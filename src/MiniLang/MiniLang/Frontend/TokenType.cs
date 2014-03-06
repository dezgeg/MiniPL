namespace MiniLang.Frontend
{
	public enum TokenType
	{
		Error = 128,
		UnterminatedStringLiteral,
		IllegalEscapeSequence,
		Newline,
		Whitespace,
		CommentStart,
		CommentEnd,
		EndOfFile,

		IntLiteral,
		StringLiteral,

		True,
		False,

		Identifier,

		DotDot,
		Semicolon,
		Colon,
		LeftParen,
		RightParen,

		For,
		In,
		Do,
		End,
		Var,
		Assert,
		Print,
		Read,

		Int,
		String,
		Bool,

		Assign,
		Not,
		LT,
		GT,
		EQ,
		NE,
		LE,
		GE,
		And,
		Div,
		Plus,
		Minus,
		Times,
	}
}

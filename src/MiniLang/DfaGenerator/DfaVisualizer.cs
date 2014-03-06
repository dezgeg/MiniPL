using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MiniLang.Frontend;

namespace MiniLang.Util
{
    public static class DfaVisualizer
    {
        public static void Visualize(byte[][] dfa, String name)
        {
            StreamWriter stream = new StreamWriter(new FileStream(name + ".dot", FileMode.Create));
            stream.WriteLine("digraph G { rankdir=LR; size=\"24,16\"; ");
            for(int srcState = 1; srcState < dfa.Length; srcState++) // for each state 'srcState'
            {
                byte[] state = dfa[srcState];
                foreach (byte destState in state.Distinct()) // for each state 'destState', where there is a transition from srcState to destState
                {
                    if (destState == 0) // do not draw transitions to error state
                        continue;
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < DfaState.DFA_TABLE_SIZE; i++) // build a string like "a-z A-Z _ 0-9", which lists all transitions from srcState to destState
                    {
                        if(state[i] != destState)
                            continue;
                        int j = i + 1;
						while (j < DfaState.DFA_TABLE_SIZE && state[j] == destState)
                            j++;
                        builder.Append(i == j - 1 ? FormatChar(i) : FormatChar(i) + "-" + FormatChar(j - 1));
                        builder.Append(' ');
                        i = j;
                    }
                    stream.WriteLine(String.Format("\"{0}\" -> \"{1}\" [label=\"{2}\"];", FormatState((byte)srcState), FormatState(destState), builder.ToString().Trim()));
                }
            }
            stream.WriteLine("}");
            stream.Close();
        }
        private static String FormatChar(int i)
        {
            char c = (char)i;
			if (c == '"' || c == '\\')
				return "\\" + c;
			else if (c == '\n')
				return "\\\\n";
			else if (c == '\r')
				return "\\\\r";
			else if (c == '\t')
				return "\\\\t";
			else if (Char.IsWhiteSpace(c) || Char.IsControl(c) || i >= 127)
				return "\\\\" + i;
            return c.ToString();
        }
        private static String FormatState(byte state)
        {
            if (Enum.IsDefined(typeof(TokenType), (int)state))
                return String.Format("{0} ({1})", state, ((TokenType)(int)state));
            return state.ToString();
        }
    }
}

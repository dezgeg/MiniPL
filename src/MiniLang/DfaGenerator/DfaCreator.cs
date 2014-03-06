using System;
using System.Linq;
using System.Collections.Generic;

using MiniLang;
using MiniLang.Frontend;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

namespace MiniLang.Util
{
    public struct DfaState
    {
        public const int DFA_TABLE_SIZE = 128;
        public static DfaState Create()
        {
            DfaState st = new DfaState();
            st.Transitions = new byte[DFA_TABLE_SIZE];
            return st;
        }
        public byte[] Transitions { get; private set; }
        public byte this[int ix]
        {
            get { return Transitions[ix]; }
            set { Transitions[ix] = value; }
        }
    }
    public class DfaCreator
    {
		public byte FIRST_RETURN_TRANS = 128;
        public string Name { get; private set; }
        public List<DfaState> States;

        public DfaCreator(string name)
        {
            Name = name;
            States = new List<DfaState>();
			this.NewState();
			for (int i = 0; i < DfaState.DFA_TABLE_SIZE; i++)
				this.States[0].Transitions[i] = FIRST_RETURN_TRANS;
        }
		public byte NewState()
        {
            int stateNum = States.Count;
            States.Add(DfaState.Create());
            return (byte)stateNum;
        }
        public byte[][] Write(StreamWriter outFile)
        {
            byte[][] arr = this.States.Select(state => state.Transitions).ToArray();


			outFile.WriteLine("\t\tprivate static readonly byte[][] " + this.Name + "DfaTable = {");
			outFile.WriteLine("\t\t\t" + String.Join(",\r\n\t\t\t", arr.Select(innerArr => "new byte[] { " + String.Join(", ", innerArr) + " }")));
			outFile.WriteLine("\t\t};");
            return arr;
        }

        public void AddTransition(byte state, byte dest, char c)
        {
            DfaState dfaState = this.States[state];
            dfaState[c] = dest;
        }
        public void AddTransitionCharRange(byte state, byte dest, char i, char j)
        {
            for (; i <= j; i++)
                this.AddTransition(state, dest, i);
        }
        public void AddMultipleTransitions(byte state, byte dest, string chars)
        {
            foreach (char c in chars)
                this.AddTransition(state, dest, c);
        }
        public void AddAcceptingTransition(byte state, Enum val)
        {
			DfaState st = this.States[state];
			for (int i = 0; i < DfaState.DFA_TABLE_SIZE; i++)
				if (st.Transitions[i] == 0)
					st.Transitions[i] = Convert.ToByte(val);
		}
        public void AddStarTransition(byte from, byte to)
        {
            DfaState st = this.States[from];
            for (int i = 1; i < DfaState.DFA_TABLE_SIZE; i++)
                if (st.Transitions[i] == 0)
                    st.Transitions[i] = to;
        }
        public void AddReturnTransition(byte state, char c, Enum val)
        {
            byte returnState = this.NewState();
            for (int i = 0; i < DfaState.DFA_TABLE_SIZE; i++)
                this.States[returnState].Transitions[i] = Convert.ToByte(val);
            this.States[state].Transitions[c] = returnState;
        }
        public void AddReturnTransition(byte state, String chars, Enum val)
        {
            foreach (char c in chars)
                this.AddReturnTransition(state, c, val);
        }
        public void AddDualCharOp(byte state, string oper, Enum val1, Enum val2)
        {
            byte temp = this.NewState();
            this.AddTransition(state, temp, oper[0]);
            this.AddReturnTransition(temp, oper[1], val2);
            this.AddAcceptingTransition(temp, val1);
        }
    }
}

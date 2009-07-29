using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public class SymbolTable
    {
        private IDictionary<string, int> dictionary;

        public SymbolTable()
        {
            this.dictionary = new Dictionary<string, int>();
            SetPredefinedSymbols();
        }

        public void AddEntry(string symbol, int address)
        {
            this.dictionary.Add(symbol, address);
        }

        public bool Contains(string symbol)
        {
            return this.dictionary.ContainsKey(symbol);
        }

        public int GetAddress(string symbol)
        {
            return this.dictionary["symbol"];
        }

        private void SetPredefinedSymbols()
        {
            dictionary.Add("SP", 0);
            dictionary.Add("LCL",1);
            dictionary.Add("ARG",2);
            dictionary.Add("THIS",3);
            dictionary.Add("THAT",4);
            dictionary.Add("SCREEN",16384);
            dictionary.Add("KBD",24576);
            for(int i=0; i<=15;i++)
            {
                dictionary.Add("R"+i,i);
            }
        }
    }

    
}

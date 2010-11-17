using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

namespace Compiler
{
    public class ExpressionCompiler
    {
        Stack<Pair<string, string>> _classTokens;
        VmWriter _vmWriter;
        SymbolTable symbolTable = new SymbolTable();

        public ExpressionCompiler(IList<Pair<string, string>> classTokensList, VmWriter vmWriter, string className)
        {
            classTokensList = classTokensList;
            this.classTokens = new Stack<Pair<string, string>>();
            this.vmWriter = vmWriter;
            this.className = className;
        }

        void CompileExpresion()
        {
            CompileTerm();
        }
    }
}

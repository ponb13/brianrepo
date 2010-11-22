//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Interfaces;

//namespace Compiler
//{
//    public class ExpressionCompiler
//    {
//        Stack<Pair<string, string>> _classTokens;
//        VmWriter _vmWriter;
//        SymbolTable symbolTable = new SymbolTable();

//        public ExpressionCompiler(IList<Pair<string, string>> classTokensList, VmWriter vmWriter, string className)
//        {
//            classTokensList = classTokensList;
//            this.classTokens = new Stack<Pair<string, string>>();
//            this.vmWriter = vmWriter;
//            this.className = className;
//        }

//        void CompileExpression()
//        {
//            CompileTerm();
//        }

//        void CompileTerm()
//        {
//            // compile opening bracket of expression if there is one see page 208 for what a term is
//            if (_classTokens.PopIfExists("("))
//            {
//                CompileExpression();
//                _classTokens.PopIfExists(")");
//                return;
//            }

//            if (_classTokens.IsIntegerConstant)
//            {
//                //this.vmWriter.WritePush(Segment.Constant, int.Parse(_classTokens.Value2));
//            }
//        }

//        bool IsTerm(Pair<string, string> token)
//        {
//            // what is a keyword constant

//            return
//                IsIntegerConstant(token) ||
//                IsStringConstant(token) ||
//                IsVarNameTerm(token) ||
//                IsSubRoutineCall(this.PeekTwoTokensDeep()||
//                IsAnExpressionKeyWord(token) 
//                IsVarNameTerm(token) ||
//                IsArrayAccessor() ||
//                IsUnaryOp(token);
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Interfaces;

namespace Compiler
{
    /// <summary> 
    /// just discover subroutines can be either method  or function (think instance and static ??) p 188 to 190
    /// talks about this and alsoobject construction and disposal, however I am confused about constructors being allowed arbitary names
    /// does the compile know it is a constructor then??
    /// </summary>
    public class CompilationEngineVm
    {
        /// <summary>
        /// use stack to store tokens 
        /// simplier than a list for the 
        /// operations that this class carries out.
        /// </summary>
        private Stack<Pair<string, string>> classTokens;

        private VmWriter vmWriter;

        private SymbolTable symbolTable = new SymbolTable();

        private string className;

        private int ifStatementCount = -1;
        private int whileStatementCount = -1;

        public CompilationEngineVm(IList<Pair<string, string>> classTokensList, VmWriter vmWriter, string className)
        {
            // reverse tokens before push onto stack (so we Pop them in the correct order!)
            classTokensList = classTokensList.Reverse().ToList();
            this.classTokens = new Stack<Pair<string, string>>();
            this.vmWriter = vmWriter;
            this.className = className;

            foreach (Pair<string, string> token in classTokensList)
            {
                this.classTokens.Push(token);
            }
        }

        /// <summary>
        /// Compiles the class.
        /// </summary>
        public XElement CompileClass()
        {
            XElement classXml = new XElement("class");

            // compile class keyword
            this.CompileTerminal();
            // compile class name
            Pair<string, string> token = this.classTokens.Pop();
            classXml.Add(new XElement(token.Value1, token.Value2,
                        new XAttribute("category", "Class")));
            this.className = token.Value2;


            // compile opening curely of class);
            this.CompileTerminal();

            this.CompileClassVarDeclaration(classXml);

            while (this.IsSubRourtineDeclaration())
            {
                this.CompileSubRoutine(classXml);
            }

            // compile class closing curely
            this.CompileTerminal();


            return classXml;
        }

        /// <summary>
        /// Compiles the sub routine.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        private void CompileSubRoutine(XElement parentElement)
        {
            // seems that labels only need to be unique inside functions / methods
            this.whileStatementCount = -1;
            this.ifStatementCount = -1;
            
            
            string subroutineName = string.Empty;
            this.symbolTable.StartNewSubroutine();

            XElement subRoutineElement = new XElement("subroutineDec");
            parentElement.Add(subRoutineElement);

            // while not the opening bracked of the method/constructor/function params
            while (this.classTokens.Peek().Value2 != "(")
            {
                Pair<string, string> token = this.classTokens.Pop();

                // finds method name i.e. if the next token is an opening bracket
                // we must be on the method name
                if (this.classTokens.Peek().Value2 == "(")
                {
                    subroutineName = token.Value2;
                    // see page 238 - don't bother adding method or class names to symbol table
                    subRoutineElement.Add(new XElement(token.Value1, token.Value2,
                        new XAttribute("category", "Subroutine")));
                }
                else
                {
                    subRoutineElement.Add(new XElement(token.Value1, token.Value2));
                }
            }

            // add the opening braket of param list
            this.CompileTerminal();

            // add the param list
            this.CompileParameterList(subRoutineElement);

            // add closing bracket of params
            this.CompileTerminal();

            // add opening curley of methodBody
            this.CompileTerminal();

            // compile sub routine var declarations (they're always first);
            this.CompileSubRoutineVariableDeclarations();

            int numberOfLocals = symbolTable.VarCount(Kind.Var);
            this.vmWriter.WriteFunction(this.className + "." + subroutineName, symbolTable.VarCount(Kind.Var));

            this.CompileSubroutineStatments();

            // compile closing curley
            this.CompileTerminal();
        }

        private void CompileSubRoutineVariableDeclarations()
        {
            while (this.IsVariableDeclaration())
            {
                this.CompileVariableDeclaration();
            }
        }

        /// <summary>
        /// Compiles the subroutine body.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileSubroutineStatments()
        {
            while (this.IsStatement())
            {
                if (this.IsStatement())
                {
                    // add statements to body
                    this.CompileStatements();
                }
            }
            // add closing curely of methodBody
        }

        /// <summary>
        /// Compile a class variable declaration
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        private void CompileClassVarDeclaration(XElement parentElement)
        {
            if (IsClassVariableDeclaration())
            {
                XElement classVariableElement = new XElement("classVarDec");
                parentElement.Add(classVariableElement);
                this.CompileClassOrSubRoutineLevelVarDeclarationAndAddToSymbolTable(classVariableElement);

                // recursively handle all class variables
                this.CompileClassVarDeclaration(parentElement);
            }
        }

        /// <summary>
        /// Compiles the variable declaration.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileVariableDeclaration()
        {
            XElement classXml = new XElement("fake");

            if (this.classTokens.Peek().Value2 == "var")
            {
                this.CompileClassOrSubRoutineLevelVarDeclarationAndAddToSymbolTable(classXml);

                // recursively call variable declaration
                this.CompileVariableDeclaration();
            }
        }

        /// <summary>
        /// Builds an identifer obj and also identifier.
        /// Only use this method for compiling class variables and subroutine variables
        /// Not parameter lists!
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private void CompileClassOrSubRoutineLevelVarDeclarationAndAddToSymbolTable(XElement parent)
        {
            Pair<string, string> kindToken = this.CompileTerminal();
            Pair<string, string> typeToken = this.CompileTerminal();
            Pair<string, string> nameToken = this.classTokens.Pop();

            Identifier identifier = new Identifier();
            identifier.Kind = (Kind)Enum.Parse(typeof(Kind), kindToken.Value2, true);
            identifier.Name = nameToken.Value2;
            identifier.Type = typeToken.Value2;
            identifier.Usage = IdentifierUsage.Defined;

            this.symbolTable.Define(identifier);

            // check for comma, i.e comma separated list of variables
            if (this.classTokens.Peek().Value2 == ",")
            {
                // compile the comma
                this.CompileTerminal();

                while (this.classTokens.Peek().Value2 != ";")
                {
                    Pair<string, string> commaSeparatedIdentifierToken = this.classTokens.Pop();

                    // set the next identifier in comma separated list to same type kind etc as fisrt
                    Identifier commaSeparatedIdentier = new Identifier();
                    commaSeparatedIdentier.Kind = identifier.Kind;
                    commaSeparatedIdentier.Type = identifier.Type;
                    commaSeparatedIdentier.Name = commaSeparatedIdentifierToken.Value2;
                    commaSeparatedIdentier.Usage = IdentifierUsage.Defined;

                    this.symbolTable.Define(commaSeparatedIdentier);

                    if (this.classTokens.Peek().Value2 == ",")
                    {
                        // compile comma
                        this.CompileTerminal();
                    }
                }
            }

            // compile the ending ;
            this.CompileTerminal();
        }

        /// <summary>
        /// Compiles the parameter list of a method/function/constructor
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        private void CompileParameterList(XElement parentElement)
        {
            // add param list as child
            XElement parameterList = new XElement("parameterList");
            parentElement.Add(parameterList);

            // add the params

            while (this.classTokens.Peek().Value2 != ")")
            {
                Pair<string, string> token = null;

                Identifier identifier = new Identifier();
                identifier.Kind = Kind.Arg;
                for (int i = 0; i <= 2; i++)
                {
                    if (this.classTokens.Peek().Value2 != ")")
                    {
                        token = this.classTokens.Pop();
                        if (i == 0)
                        {
                            identifier.Type = token.Value2;
                            parameterList.Add(new XElement(token.Value1, token.Value2));
                        }
                        else if (i == 1)
                        {
                            identifier.Name = token.Value2;
                            identifier.Usage = IdentifierUsage.Defined;
                            this.symbolTable.Define(identifier);
                        }
                        else if (i == 2)
                        {
                            //add the comma
                            parameterList.Add(new XElement(token.Value1, token.Value2));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Compiles statments.
        /// </summary>
        private void CompileStatements()
        {
            if (this.IsStatement())
            {

                // keep adding statements to this statement element
                while (this.IsStatement())
                {
                    if (this.IsLetStatement())
                    {
                        this.CompileLet();
                    }
                    else if (this.IsIfStatement())
                    {
                        this.CompileIf();
                    }
                    else if (this.IsWhileStatement())
                    {
                        this.CompileWhile();
                    }
                    else if (this.IsDoStatement())
                    {
                        this.CompileDoStatement();
                    }
                    else if (this.IsReturnStatement())
                    {
                        this.CompileReturn();
                    }
                }
            }
        }

        /// <summary>
        /// Compiles a do statement.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileDoStatement()
        {
            //'do' this.subRoutineCall ';' i.e Main.main == this.Main if we're in main
            // do class.method
            string classNameOfMethodToBeCalled = string.Empty;
            string nameOfMethod = string.Empty;

            // compile do keyword
            this.CompileTerminal();

            // compile identifier
            classNameOfMethodToBeCalled = this.CompileTerminal().Value2;

            // compile the sub routine call
            this.CompileSubRoutineCall(classNameOfMethodToBeCalled);

            // its a do statement so we know its void -see p.235
            vmWriter.WritePop(Segment.Temp, 0);

            //compile ;);
            this.CompileTerminal();
        }

        /// <summary>
        /// Compiles a let statment.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileLet()
        {
            //'let' varName ('[' expression ']')? '=' expression ';'

            // compile let keyword
            this.CompileTerminal();
            // compile identifier
            //this.CompileTerminal(letElement);

            // get the identifier on the left side of the = (i.e. let SomeIden = exp);
            // do the vm write after expression compiles
            Pair<string, string> token = this.classTokens.Pop();
            Identifier letIdentifier = this.symbolTable.GetIdentifierByName(token.Value2);

            // compile array accessor [ expression ] (if there is one)));
            if (this.CompileTokenIfExists("["))
            {
                this.CompileExpression();
                //compile closing ]
                this.CompileTerminal();
            }

            // compile =
            this.CompileTerminal();

            // compile opening bracket of expression if there is one
            // in this instance unlike array accessor above we might have an expression even if there
            // isn't any brackets
            this.CompileTokenIfExists("(");

            // compile expression
            this.CompileExpression();
            
            vmWriter.WritePopIdentifier(letIdentifier);

            // compile closing bracket of expression if there is one
            this.CompileTokenIfExists(")");

            //compile ;
            this.CompileTerminal();
        }

        /// <summary>
        /// Compiles an if statement.
        /// </summary>
        private void CompileIf()
        {
            this.ifStatementCount++;

            string ifTrueLabel = "IF_TRUE" + this.ifStatementCount;
            string ifFalseLabel = "IF_FALSE" + this.ifStatementCount;
            string endIf = "IF_END" + this.ifStatementCount;

            // compile if keyword
            this.CompileTerminal();
            // compile  opening bracket
            this.CompileTerminal();
            // compile expression
            this.CompileExpression();
            // compile closing bracket
            this.CompileTerminal();
            // compile opening curly brace
            this.CompileTerminal();

            this.vmWriter.WriteIf(ifTrueLabel);
            this.vmWriter.WriteGoto(ifFalseLabel);
            this.vmWriter.WriteLabel(ifTrueLabel);
            // true statements
            // compile the true statements inside the if
            this.CompileStatements();
            this.vmWriter.WriteGoto(endIf);

            // compile closing curly brace
            this.CompileTerminal();

            // compile else statement if there is one
            if (this.classTokens.Peek().Value2 == "else")
            {
                // this maybe incorrect, may should create elseStatement and add children??
                // compile the else keyword
                this.CompileTerminal();
                // compile opening curly brace
                this.CompileTerminal();

                this.vmWriter.WriteLabel(ifFalseLabel);
                // compile statments
                this.CompileStatements();
                // compile closing curly brace
                this.CompileTerminal();
            }
            else
            {
                // crap hack - don't really handle difference between if and if else very well
                // need this crap if false label to be output .
                this.vmWriter.WriteLabel(ifFalseLabel);
            }

            this.vmWriter.WriteLabel(endIf);
        }

        /// <summary>
        /// Compiles a while loop.
        /// </summary>
        private void CompileWhile()
        {
            this.whileStatementCount++;

            string whileExpressionLabel = "WHILE_EXP" + this.whileStatementCount;
            string whileEndLabel = "WHILE_END" + this.whileStatementCount;

            vmWriter.WriteLabel(whileExpressionLabel);

            // compile the while keyword);
            this.CompileTerminal();
            // compile the opening bracket 
            this.CompileTerminal();
            //compile the expression
            this.CompileExpression();

            // if not true goto end of while
            vmWriter.WriteArithmetic(ArithmeticCommand.Not);
            vmWriter.WriteIf(whileEndLabel);

            //compile the closing bracket);
            this.CompileTerminal();
            //compile the opening curly bracket
            this.CompileTerminal();
            //compile statements inside while 
            this.CompileStatements();
            //compile the closing curly bracket
            this.CompileTerminal();

            this.vmWriter.WriteGoto(whileExpressionLabel);

            this.vmWriter.WriteLabel(whileEndLabel);
        }

        private void CompileReturn()
        {
            // compile return keyword
            this.CompileTerminal();

            // vm writer return doesn wite full expression after return yet

            string nextTokenChar = this.classTokens.Peek().Value2;

            // compile return expression
            if (nextTokenChar != ";")
            {
                this.CompileTokenIfExists("(");
                this.CompileExpression();
                this.CompileTokenIfExists(")");
            }
            else
            {
                // no expression after return means this is a void method see p235
                vmWriter.WritePush(Segment.Constant, 0);
            }

            vmWriter.WriteReturn();

            // compile the ;
            this.CompileTerminal();
        }

        private Pair<string, string> CompileInUseIdentifier()
        {
            Pair<string, string> token = this.classTokens.Pop();

            Identifier identifier = this.symbolTable.GetIdentifierByName(token.Value2);

            if (identifier != null)
            {
                identifier.Usage = IdentifierUsage.InUse;
            }
            else
            {
                // ignore - this identifer has not been found in symbol table - has not been declared
                // hacky way of ignoring method names, and other "identifiers"
            }

            return token;
        }

        /// <summary>
        /// Compiles an expression.
        /// </summary>
        private void CompileExpression()
        {
            // TODO this needs refactored -  see page 209
            this.CompileTerm();

            if (this.IsOperator())
            {
                ArithmeticCommand arithmeticCommand = this.CompileArithmeticCommand();
                this.CompileTerm();
                vmWriter.WriteArithmetic(arithmeticCommand);

                if (this.IsOperator())
                {
                    ArithmeticCommand arithmeticCommand2 = this.CompileArithmeticCommand();
                    this.CompileTerm();
                    vmWriter.WriteArithmetic(arithmeticCommand2);
                    this.CompileExpression();
                }
            }
        }

        private ArithmeticCommand CompileArithmeticCommand()
        {
            ArithmeticCommand vmOp = ArithmeticCommand.Add;
            Pair<string, string> operatorToken = this.CompileExpressionTerminal();

            switch (operatorToken.Value2)
            {
                case ("+"):
                    {
                        vmOp = ArithmeticCommand.Add;
                        break;
                    }
                case ("-"):
                    {
                        vmOp = ArithmeticCommand.Sub;
                        break;
                    }
                case ("*"):
                    {
                        vmOp = ArithmeticCommand.Mult;
                        break;
                    }
                case ("/"):
                    {
                        vmOp = ArithmeticCommand.Divide;
                        break;
                    }
                case ("&"):
                    {
                        vmOp = ArithmeticCommand.And;
                        break;
                    }
                case ("|"):
                    {
                        vmOp = ArithmeticCommand.And;
                        break;
                    }
                case (">"):
                    {
                        vmOp = ArithmeticCommand.Gt;
                        break;
                    }
                case ("<"):
                    {
                        vmOp = ArithmeticCommand.Lt;
                        break;
                    }
                case ("="):
                    {
                        vmOp = ArithmeticCommand.Eq;
                        break;
                    }
                
            }

            return vmOp;
        }

        /// <summary>
        /// Compiles a terminal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private Pair<string, string> CompileTerminal()
        {
            Pair<string, string> terminal = null;
            if (this.classTokens.Count > 0)
            {
                // handle tagging in use identifiers
                if (this.classTokens.Peek().Value1 == "Identifier" &&
                    (this.symbolTable.GetIdentifierByName(this.classTokens.Peek().Value2) != null))
                {

                    terminal = this.CompileInUseIdentifier();
                }
                else
                {
                    terminal = this.classTokens.Pop();
                }
            }
            return terminal;
        }

       

        private bool IsIntegerConstant(Pair<string, string> token)
        {
            int number = 0;
            
            return int.TryParse(token.Value2, out number);
        }

        /// <summary>
        /// 
        /// </summary>
        private Pair<string, string> CompileExpressionTerminal()
        {
            // todo so far can push numbers
            Pair<string, string> token = this.classTokens.Pop();

            // maybe an identifier 
            Identifier identifier = this.symbolTable.GetIdentifierByName(token.Value2);

            if (this.IsIntegerConstant(token))
            {
                this.vmWriter.WritePush(Segment.Constant, int.Parse(token.Value2));
            }
            else if (identifier != null)
            {
                this.vmWriter.WritePush(identifier.Segment, identifier.Index);
            }

            return token;

        }

        private void CompileTerm()
        {
            // compile the first part no matter what
            Pair<string, string> peekedToken = this.classTokens.Peek();

            if (TokenIsAnExpressionTerm(peekedToken))
            {
                Pair<string, string> compiledToken = this.CompileExpressionTerminal();
                if (peekedToken.Value2 == "[")
                {
                    // if array accessor
                    // compile the [
                    this.CompileTerminal();
                    // compile expression inside []
                    this.CompileExpression();
                    // compile closing ]
                    this.CompileTerminal();
                }
                    //check and compile '('expression')'
                else if (peekedToken.Value2 == "(")
                {
                    this.CompileExpression();
                    // compile closing )
                    this.CompileTerminal();
                }
                else if (peekedToken.Value2 == "-" || peekedToken.Value2 == "~")
                {
                    this.CompileTerm();
                    if (peekedToken.Value2 == "-")
                    {
                        vmWriter.WriteArithmetic(ArithmeticCommand.Neg);
                    }
                    if (peekedToken.Value2 == "~")
                    {
                        vmWriter.WriteArithmetic(ArithmeticCommand.Not);
                    }
                }
                else if (peekedToken.Value2 == "true")
                {
                    vmWriter.WritePush(Segment.Constant, 0);
                    vmWriter.WriteArithmetic(ArithmeticCommand.Not);

                }
                else if (peekedToken.Value2 == "false")
                {
                    vmWriter.WritePush(Segment.Constant, 0);
                }
                else if (this.IsSubRoutineCall(this.classTokens.Peek()))
                {
                    this.CompileSubRoutineCall(compiledToken.Value2);
                }
            }
        }

        private bool TokenIsAnExpressionTerm(Pair<string,string> token)
        {
            // see page 209
            // 'stringConstant' 
            // 'unaryOp Term'
            

            return (IsIntegerConstant(token) || IsAnExpressionKeyWord(token) || IsVarNameTerm(token) || IsArrayAccessor() ||
                    IsSubRoutineCall(this.PeekTwoTokensDeep()) || this.classTokens.Peek().Value2 == ("(") || this.IsUnaryOp(token));
        }

        private bool IsVarNameTerm(Pair<string, string> token)
        {
            Identifier identifier = this.symbolTable.GetIdentifierByName(token.Value2);
            return identifier != null;
        }

        private bool IsAnExpressionKeyWord(Pair<string,string> token)
        {
            // see p 209 in expression box KeyWordConstant
            return (token.Value2 == "true" || token.Value2 == "false" || token.Value2 == "null" ||
                    token.Value2 == "this");
        }

        private bool IsUnaryOp(Pair<string,string> token)
        {
            return token.Value2 == "-" || token.Value2 == "~";
        }

        private bool IsArrayAccessor()
        {
            
            
            return this.classTokens.Peek().Value2 == "[";
        }

        private void CompileSubRoutineCall(string classNameOrFunctionName)
        {
            // hack on class name - as we can have className.Method
            // or just Method() pass in the className so the VmWriter can use it


            // subname'('expressionList')' 
            // OR
            // classOrVarName.subname'('expressionList')' 

            //subname or classOrVarName will have already been compiled

            if (this.classTokens.Peek().Value2 == "(")
            {
                // compile opening bracket
                this.CompileTerminal();
                // compile the expression list
                int numberOfArgsPushed = this.CompileExpressionList();
                // compile closing bracket
                this.CompileTerminal();

                this.vmWriter.WriteFunction(this.className + "." + classNameOrFunctionName, numberOfArgsPushed);
            }
            else if (this.classTokens.Peek().Value2 == ".")
            {
                // compile the dot
                this.CompileTerminal();
                // compile subName
                string subRoutineName = this.CompileTerminal().Value2;
                // compile opening bracket
                this.CompileTerminal();
                // compile the expression list                    
                int numberOfArgsPushed = this.CompileExpressionList();
                // compile closing bracket
                this.CompileTerminal();

                // this should translate to className
                classNameOrFunctionName = (classNameOrFunctionName == "this") ? this.className : classNameOrFunctionName;

                this.vmWriter.WriteCall(classNameOrFunctionName + "." + subRoutineName, numberOfArgsPushed);
            }
        }

        /// <summary>
        /// Compiles an expression list.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private int CompileExpressionList()
        {
            // vmWriter - compiling a method call needs number of args pushed
            int expressionCount = 0;

            // check for empty brackets
            if (this.classTokens.Peek().Value2 != ")")
            {
                this.CompileExpression();

                expressionCount++;
                while (this.classTokens.Peek().Value2 == ",")
                {
                    // comile comma
                    this.CompileTerminal();

                    //compile expression
                    this.CompileExpression();
                    expressionCount++;
                }
            }
            return expressionCount;
        }

        /// <summary>
        /// Determines whether [is class variable declaration].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is class variable declaration]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsClassVariableDeclaration()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && (peekedToken.Value2 == "field" || peekedToken.Value2 == "static");
        }

        private bool IsSubRoutineCall(Pair<string,string> token)
        {
            bool result = false;

            if (token.Value2 == "." || token.Value2 == "(")
            {
                result = true;
            }

            return result;
        }

        private Pair<string,string> PeekTwoTokensDeep()
        {
            Pair<string,string> topToken = this.classTokens.Pop();
            Pair<string, string> secondTopToken = this.classTokens.Peek();

            this.classTokens.Push(topToken);

            return secondTopToken;
        }

        private bool IsSubRourtineDeclaration()
        {
            bool result = false;
            if (this.classTokens.Count > 0)
            {
                Pair<string, string> peekedToken = this.classTokens.Peek();
                result = peekedToken.Value1 == StringConstants.keyword && (peekedToken.Value2 == "function" || peekedToken.Value2 == "constructor" || peekedToken.Value2 == "method");
            }
            return result;
        }

        private bool IsExpression()
        {
            throw new NotImplementedException();
        }

        private bool IsVariableDeclaration()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "var";
        }

        private bool IsStatement()
        {
            return (IsLetStatement() || IsIfStatement() || IsDoStatement() || IsReturnStatement() || IsWhileStatement());
        }

        private bool IsLetStatement()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "let";
        }

        private bool IsIfStatement()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "if";
        }

        private bool IsDoStatement()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "do"; ;
        }

        private bool IsReturnStatement()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "return";
        }

        private bool IsWhileStatement()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "while";
        }

        private bool IsOperator()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            Match match = Regex.Match(peekedToken.Value2, @"[+|\-|*|/|&|<|>|=]", RegexOptions.Compiled);

            return match.Success;
        }

        /// <summary>
        /// Checks if the character matches the next token in the stack
        /// if it does it pops it and compiles it.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="characterToLookFor">The character to look for.</param>
        /// <returns></returns>
        private bool CompileTokenIfExists(string characterToLookFor)
        {
            bool result = false;
            if (this.classTokens.Peek().Value2 == characterToLookFor)
            {
                this.CompileTerminal();
                result = true;
            }
            return result;
        }
    }
}

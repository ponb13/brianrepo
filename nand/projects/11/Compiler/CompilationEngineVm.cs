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
    //// can't work out why indexing seems to 1 out on some locals...
    ///
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

        private int ifStatementCount = 0;
        private int whileStatementCount = 0;

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
            this.CompileTerminal(classXml);
            // compile class name
            Pair<string, string> token = this.classTokens.Pop();
            classXml.Add(new XElement(token.Value1, token.Value2,
                        new XAttribute("category", "Class")));
            this.className = token.Value2;


            // compile opening curely of class);
            this.CompileTerminal(classXml);

            this.CompileClassVarDeclaration(classXml);

            while (this.IsSubRourtineDeclaration())
            {
                this.CompileSubRoutine(classXml);
            }

            // compile class closing curely
            this.CompileTerminal(classXml);


            return classXml;
        }

        /// <summary>
        /// Compiles the sub routine.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        private void CompileSubRoutine(XElement parentElement)
        {
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
            this.CompileTerminal(subRoutineElement);

            // add the param list
            this.CompileParameterList(subRoutineElement);

            // add closing bracket of params
            this.CompileTerminal(subRoutineElement);

            // add opening curley of methodBody
            this.CompileTerminal(subRoutineElement);

            // compile sub routine var declarations (they're always first);
            this.CompileSubRoutineVariableDeclarations();

            int numberOfLocals = symbolTable.VarCount(Kind.Var);
            this.vmWriter.WriteFunction(this.className + "." + subroutineName, symbolTable.VarCount(Kind.Var));

            this.CompileSubroutineStatments();

            // compile closing curley
            this.CompileTerminal(subRoutineElement);
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
            XElement fakeBody = new XElement("subroutineBody");

            while (this.IsStatement())
            {
                if (this.IsStatement())
                {
                    // add statements to body
                    this.CompileStatements(fakeBody);
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
            Pair<string, string> kindToken = this.CompileTerminal(parent);
            Pair<string, string> typeToken = this.CompileTerminal(parent);
            Pair<string, string> nameToken = this.classTokens.Pop();

            Identifier identifier = new Identifier();
            identifier.Kind = (Kind)Enum.Parse(typeof(Kind), kindToken.Value2, true);
            identifier.Name = nameToken.Value2;
            identifier.Type = typeToken.Value2;
            identifier.Usage = IdentifierUsage.Defined;

            this.symbolTable.Define(identifier);

            this.CreateIdentifierElementWithAttributes(parent, identifier, nameToken);

            // check for comma, i.e comma separated list of variables
            if (this.classTokens.Peek().Value2 == ",")
            {
                // compile the comma
                this.CompileTerminal(parent);

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

                    this.CreateIdentifierElementWithAttributes(parent, commaSeparatedIdentier, commaSeparatedIdentifierToken);

                    if (this.classTokens.Peek().Value2 == ",")
                    {
                        // compile comma
                        this.CompileTerminal(parent);
                    }
                }
            }

            // compile the ending ;
            this.CompileTerminal(parent);
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
                            this.CreateIdentifierElementWithAttributes(parameterList, identifier, token);
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

        private void CreateIdentifierElementWithAttributes(XElement parent, Identifier identifier, Pair<string, string> identifierToken)
        {
            // quick hack with the category - if the identifier is not a method or a class name the category will be the same as kind
            // see page p243's confusing spec.
            parent.Add(new XElement(identifierToken.Value1, identifierToken.Value2,
                              new XAttribute("type", identifier.Type),
                              new XAttribute("usage", identifier.Usage),
                              new XAttribute("kind", identifier.Kind),
                              new XAttribute("category", identifier.Kind),
                              new XAttribute("index", identifier.Index)));

        }

        /// <summary>
        /// Compiles statments.
        /// </summary>
        private void CompileStatements(XElement parentElement)
        {
            if (this.IsStatement())
            {
                XElement statementsElement = new XElement("statements");
                parentElement.Add(statementsElement);

                // keep adding statements to this statement element
                while (this.IsStatement())
                {
                    if (this.IsLetStatement())
                    {
                        this.CompileLet(statementsElement);
                    }
                    else if (this.IsIfStatement())
                    {
                        this.CompileIf(statementsElement);
                    }
                    else if (this.IsWhileStatement())
                    {
                        this.CompileWhile(statementsElement);
                    }
                    else if (this.IsDoStatement())
                    {
                        this.CompileDoStatement(statementsElement);
                    }
                    else if (this.IsReturnStatement())
                    {
                        this.CompileReturn(statementsElement);
                    }
                }
            }
        }

        /// <summary>
        /// Compiles a do statement.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileDoStatement(XElement parent)
        {
            //'do' this.subRoutineCall ';' i.e Main.main == this.Main if we're in main
            // do class.method
            string classNameOfMethodToBeCalled = string.Empty;
            string nameOfMethod = string.Empty;

            XElement doElement = new XElement("doStatement");
            parent.Add(doElement);

            // compile do keyword
            this.CompileTerminal(doElement);

            // compile identifier
            classNameOfMethodToBeCalled = this.CompileTerminal(doElement).Value2;

            // compile the sub routine call
            this.CompileSubRoutineCall(doElement, classNameOfMethodToBeCalled);

            // its a do statement so we know its void -see p.235
            vmWriter.WritePop(Segment.Temp, 0);

            //compile ;);
            this.CompileTerminal(doElement);
        }

        /// <summary>
        /// Compiles a let statment.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileLet(XElement parent)
        {
            //'let' varName ('[' expression ']')? '=' expression ';'

            XElement letElement = new XElement("letStatement");
            parent.Add(letElement);

            // compile let keyword
            this.CompileTerminal(letElement);
            // compile identifier
            //this.CompileTerminal(letElement);

            // get the identifier on the left side of the = (i.e. let SomeIden = exp);
            // do the vm write after expression compiles
            Pair<string, string> token = this.classTokens.Pop();
            Identifier letIdentifier = this.symbolTable.GetIdentifierByName(token.Value2);

            // compile array accessor [ expression ] (if there is one)));
            if (this.CompileTokenIfExists(letElement, "["))
            {
                this.CompileExpression(letElement);
                //compile closing ]
                this.CompileTerminal(letElement);
            }

            // compile =
            this.CompileTerminal(letElement);

            // compile opening bracket of expression if there is one
            // in this instance unlike array accessor above we might have an expression even if there
            // isn't any brackets
            this.CompileTokenIfExists(parent, "(");

            // compile expression
            this.CompileExpression(letElement);

            if (letIdentifier.IdentifierScope == Scope.MethodLevel)
            {
                vmWriter.WritePop(Segment.Local, letIdentifier.Index);
            }

            // compile closing bracket of expression if there is one
            this.CompileTokenIfExists(parent, ")");

            //compile ;
            this.CompileTerminal(letElement);
        }

        /// <summary>
        /// Compiles an if statement.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileIf(XElement parent)
        {
            XElement ifElement = new XElement("ifStatement");
            parent.Add(ifElement);

            string ifTrueLabel = "IF_TRUE" + this.ifStatementCount;
            string ifFalseLabel = "IF_FALSE" + this.ifStatementCount;
            string endIf = "IF_END" + this.ifStatementCount;

            // compile if keyword
            this.CompileTerminal(ifElement);
            // compile  opening bracket
            this.CompileTerminal(ifElement);
            // compile expression
            this.CompileExpression(ifElement);
            // compile closing bracket
            this.CompileTerminal(ifElement);
            // compile opening curly brace
            this.CompileTerminal(ifElement);

            this.vmWriter.WriteIf(ifTrueLabel);
            this.vmWriter.WriteGoto(ifFalseLabel);
            this.vmWriter.WriteLabel(ifTrueLabel);
            // true statements
            // compile the true statements inside the if
            this.CompileStatements(ifElement);
            this.vmWriter.WriteGoto(endIf);

            // compile closing curly brace
            this.CompileTerminal(ifElement);

            // compile else statement if there is one
            if (this.classTokens.Peek().Value2 == "else")
            {
                // this maybe incorrect, may should create elseStatement and add children??
                // compile the else keyword
                this.CompileTerminal(ifElement);
                // compile opening curly brace
                this.CompileTerminal(ifElement);

                this.vmWriter.WriteLabel(ifFalseLabel);
                // compile statments
                this.CompileStatements(ifElement);
                // compile closing curly brace
                this.CompileTerminal(ifElement);
            }
            else
            {
                // crap hack - don't really handle difference between if and if else very well
                // need this crap if false label to be output .
                this.vmWriter.WriteLabel(ifFalseLabel);
            }

            this.vmWriter.WriteLabel(endIf);
            this.ifStatementCount++;
        }

        /// <summary>
        /// Compiles a while loop.
        /// </summary>
        private void CompileWhile(XElement parent)
        {
            XElement whileElement = new XElement("whileStatement");
            parent.Add(whileElement);

            string whileExpressionLabel = "WHILE_EXP" + this.whileStatementCount;
            string whileEndLabel = "WHILE_END" + this.whileStatementCount;

            vmWriter.WriteLabel(whileExpressionLabel);

            // compile the while keyword);
            this.CompileTerminal(whileElement);
            // compile the opening bracket 
            this.CompileTerminal(whileElement);
            //compile the expression
            this.CompileExpression(whileElement);

            // if not true goto end of while
            vmWriter.WriteArithmetic(ArithmeticCommand.Not);
            vmWriter.WriteIf(whileEndLabel);

            //compile the closing bracket);
            this.CompileTerminal(whileElement);
            //compile the opening curly bracket
            this.CompileTerminal(whileElement);
            //compile statements inside while 
            this.CompileStatements(whileElement);
            //compile the closing curly bracket
            this.CompileTerminal(whileElement);

            this.vmWriter.WriteGoto(whileExpressionLabel);

            this.vmWriter.WriteLabel(whileEndLabel);
            this.whileStatementCount++;
        }

        private void CompileReturn(XElement parent)
        {
            XElement returnElement = new XElement("returnStatement");
            parent.Add(returnElement);

            // compile return keyword
            this.CompileTerminal(returnElement);

            // vm writer return doesn wite full expression after return yet

            string nextTokenChar = this.classTokens.Peek().Value2;

            // compile return expression
            if (nextTokenChar != ";")
            {
                this.CompileTokenIfExists(returnElement, "(");
                this.CompileExpression(returnElement);
                this.CompileTokenIfExists(returnElement, ")");
            }
            else
            {
                // no expression after return means this is a void method see p235
                vmWriter.WritePush(Segment.Constant, 0);
            }

            vmWriter.WriteReturn();

            // compile the ;
            this.CompileTerminal(returnElement);
        }

        private Pair<string, string> CompileInUseIdentifier(XElement parent)
        {
            Pair<string, string> token = this.classTokens.Pop();

            Identifier identifier = this.symbolTable.GetIdentifierByName(token.Value2);

            if (identifier != null)
            {
                identifier.Usage = IdentifierUsage.InUse;
                this.CreateIdentifierElementWithAttributes(parent, identifier, token);
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
        /// <param name="parent">The parent.</param>
        private void CompileExpression(XElement parent)
        {
            XElement expressionElement = new XElement("expression");
            parent.Add(expressionElement);

            this.CompileTerm(expressionElement);

            if (this.IsOperator())
            {
                ArithmeticCommand arithmeticCommand = this.CompileArithmeticCommand();
                this.CompileTerm(expressionElement);
                vmWriter.WriteArithmetic(arithmeticCommand);
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
            }

            return vmOp;
        }

        /// <summary>
        /// Compiles a terminal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private Pair<string, string> CompileTerminal(XElement parent)
        {
            Pair<string, string> terminal = null;
            if (this.classTokens.Count > 0)
            {
                // handle tagging in use identifiers
                if (this.classTokens.Peek().Value1 == "Identifier" &&
                    (this.symbolTable.GetIdentifierByName(this.classTokens.Peek().Value2) != null))
                {

                    terminal = this.CompileInUseIdentifier(parent);
                }
                else
                {
                    terminal = this.classTokens.Pop();
                    parent.Add(new XElement(terminal.Value1, terminal.Value2));
                }
            }
            return terminal;
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

            if (this.TokenIsANumber(token))
            {
                this.vmWriter.WritePush(Segment.Constant, int.Parse(token.Value2));
            }
            else if (identifier != null)
            {
                this.vmWriter.WritePush(identifier.Segment, identifier.Index);
            }

            return token;

        }

        private bool TokenIsANumber(Pair<string, string> token)
        {
            int number = 0;
            int.TryParse(token.Value2, out number);

            return number != 0;
        }

        private void CompileTerm(XElement parent)
        {
            XElement termElement = new XElement("term");
            parent.Add(termElement);

            // compile the first part no matter what
            Pair<string, string> peekedToken = this.classTokens.Peek();
            Pair<string, string> compiledToken = this.CompileExpressionTerminal();
            if (peekedToken.Value2 == "[")
            {
                // if array accessor
                // compile the [
                this.CompileTerminal(termElement);
                // compile expression inside []
                this.CompileExpression(termElement);
                // compile closing ]
                this.CompileTerminal(termElement);
            }
            //check and compile '('expression')'
            else if (peekedToken.Value2 == "(")
            {
                this.CompileExpression(termElement);
                // compile closing )
                this.CompileTerminal(termElement);
            }
            else if (peekedToken.Value2 == "-" || peekedToken.Value2 == "~")
            {
                this.CompileTerm(termElement);
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
            else if(peekedToken.Value2 == "false")
            {
                vmWriter.WritePush(Segment.Constant, 0);
            }
            else if (this.IsSubRoutineCall())
            {
                this.CompileSubRoutineCall(termElement, compiledToken.Value2);
            }
        }

        private void CompileSubRoutineCall(XElement parent, string classNameOrFunctionName)
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
                this.CompileTerminal(parent);
                // compile the expression list
                int numberOfArgsPushed = this.CompileExpressionList(parent);
                // compile closing bracket
                this.CompileTerminal(parent);

                this.vmWriter.WriteFunction(this.className + "." + classNameOrFunctionName, numberOfArgsPushed);
            }
            else if (this.classTokens.Peek().Value2 == ".")
            {
                // compile the dot
                this.CompileTerminal(parent);
                // compile subName
                string subRoutineName = this.CompileTerminal(parent).Value2;
                // compile opening bracket
                this.CompileTerminal(parent);
                // compile the expression list                    
                int numberOfArgsPushed = this.CompileExpressionList(parent);
                // compile closing bracket
                this.CompileTerminal(parent);

                // this should translate to className
                classNameOrFunctionName = (classNameOrFunctionName == "this") ? this.className : classNameOrFunctionName;

                this.vmWriter.WriteCall(classNameOrFunctionName + "." + subRoutineName, numberOfArgsPushed);
            }
        }

        /// <summary>
        /// Compiles an expression list.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private int CompileExpressionList(XElement parent)
        {
            // vmWriter - compiling a method call needs number of args pushed
            int expressionCount = 0;
            XElement expressionListElement = new XElement("expressionList");
            parent.Add(expressionListElement);

            // check for empty brackets
            if (this.classTokens.Peek().Value2 != ")")
            {
                this.CompileExpression(expressionListElement);

                expressionCount++;
                while (this.classTokens.Peek().Value2 == ",")
                {
                    // comile comma
                    this.CompileTerminal(expressionListElement);

                    //compile expression
                    this.CompileExpression(expressionListElement);
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

        private bool IsSubRoutineCall()
        {
            bool result = false;

            Pair<string, string> peekedToken = this.classTokens.Peek();

            if (peekedToken.Value2 == "." || peekedToken.Value2 == "(")
            {
                result = true;
            }

            return result;
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
            Match match = Regex.Match(peekedToken.Value2, @"[+|\-|*|/|&|<|>|=|~]", RegexOptions.Compiled);

            return match.Success;
        }

        /// <summary>
        /// Checks if the character matches the next token in the stack
        /// if it does it pops it and compiles it.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="characterToLookFor">The character to look for.</param>
        /// <returns></returns>
        private bool CompileTokenIfExists(XElement parent, string characterToLookFor)
        {
            bool result = false;
            if (this.classTokens.Peek().Value2 == characterToLookFor)
            {
                this.CompileTerminal(parent);
                result = true;
            }
            return result;
        }
    }
}

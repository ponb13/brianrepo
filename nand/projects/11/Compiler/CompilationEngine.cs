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
    /// 
    /// </summary>
    public class CompilationEngine
    {
        /// <summary>
        /// use stack to store tokens 
        /// simplier than a list for the 
        /// operations that this class carries out.
        /// </summary>
        private Stack<Pair<string, string>> classTokens;

        private SymbolTable symbolTable = new SymbolTable();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationEngine"/> class.
        /// </summary>
        /// <param name="classtokensList">The classtokens list.</param>
        public CompilationEngine(IList<Pair<string, string>> classTokensList)
        {
            // reverse tokens before push onto stack (so we Pop them in the correct order!)
            classTokensList = classTokensList.Reverse().ToList();
            this.classTokens = new Stack<Pair<string, string>>();

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
            // compiler class name
            this.CompileTerminal(classXml);
            // compile opening curely of class
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
            XElement subRoutineElement = new XElement("subroutineDec");
            parentElement.Add(subRoutineElement);

            // while not the opening bracked of the method/constructor/function params
            while (this.classTokens.Peek().Value2 != "(")
            {
                Pair<string, string> token = this.classTokens.Pop();
                subRoutineElement.Add(new XElement(token.Value1, token.Value2));
            }

            // add the opening braket of param list
            this.CompileTerminal(subRoutineElement);

            // add the param list
            this.CompileParameterList(subRoutineElement);

            // add closing bracket of params
            this.CompileTerminal(subRoutineElement);

            // compile sub routine body
            this.CompileSubroutineBody(subRoutineElement);
        }

        /// <summary>
        /// Compiles the subroutine body.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileSubroutineBody(XElement parent)
        {
            XElement subRoutineBody = new XElement("subroutineBody");
            parent.Add(subRoutineBody);

            // add opening curley of methodBody
            this.CompileTerminal(subRoutineBody);

            this.CompileVariableDeclaration(subRoutineBody);

            // while is statement || expression || variableDeclaration
            while (this.IsStatement() || this.IsVariableDeclaration()) // TODO or is expression or variable declaraiotn
            {
                if (this.IsStatement())
                {
                    // add statements to body
                    this.CompileStatements(subRoutineBody);
                }
                else if (this.IsVariableDeclaration())
                {
                    this.CompileVariableDeclaration(subRoutineBody);
                }
            }

            // add closing curely of methodBody
            this.CompileTerminal(subRoutineBody);
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

                this.CompileIdentifierDeclarationAndAddToSymbolTable(classVariableElement);

                // recursively handle all class variables
                this.CompileClassVarDeclaration(parentElement);
            }
        }

        /// <summary>
        /// Compiles the variable declaration.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileVariableDeclaration(XElement parent)
        {
            if (this.classTokens.Peek().Value2 == "var")
            {
                XElement varDec = new XElement("varDec");
                parent.Add(varDec);

                this.CompileIdentifierDeclarationAndAddToSymbolTable(varDec);

                // recursively call variable declaration
                this.CompileVariableDeclaration(parent);
            }
        }

        /// <summary>
        /// Builds an identifer obj and also identifier.
        /// Only use this method for compiling class variables and subroutine variables
        /// Not parameter lists!
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private void CompileIdentifierDeclarationAndAddToSymbolTable(XElement parent)
        {
            Pair<string, string> kindToken = this.CompileTerminal(parent);
            Pair<string, string> typeToken = this.CompileTerminal(parent);
            Pair<string, string> nameToken = this.classTokens.Pop();

            Identifier identifier = new Identifier();
            identifier.Kind = (Kind)Enum.Parse(typeof(Kind), kindToken.Value2, true);
            identifier.Name = nameToken.Value2;
            identifier.Type = typeToken.Value2;

            this.symbolTable.Define(identifier);

            parent.Add(new XElement(nameToken.Value1, nameToken.Value2,
                           new XAttribute("type", identifier.Type),
                           new XAttribute("usedOrDefined", "defined"),
                           new XAttribute("kind", identifier.Kind)));

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

                    this.symbolTable.Define(commaSeparatedIdentier);

                    parent.Add(new XElement(commaSeparatedIdentifierToken.Value1, commaSeparatedIdentifierToken.Value2,
                        new XAttribute("type", commaSeparatedIdentier.Type),
                        new XAttribute("usedOrDefined", "defined"),
                        new XAttribute("kind", commaSeparatedIdentier.Kind)));

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
                            parameterList.Add(new XElement(token.Value1, token.Value2,
                              new XAttribute("type", identifier.Type),
                              new XAttribute("usedOrDefined", "defined"),
                              new XAttribute("kind",identifier.Kind)));
                            // add to symbol table
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
            //'do' subRoutineCall ';'

            XElement doElement = new XElement("doStatement");
            parent.Add(doElement);

            // compile do keyword
            this.CompileTerminal(doElement);

            // compile identifier
            this.CompileTerminal(doElement);

            // compile the sub routine call
            this.CompileSubRoutineCall(doElement);

            //compile ;
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
            this.CompileTerminal(letElement);

            // compile array accessor [ expression ] (if there is one)
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
            //compile the statement inside the if
            this.CompileStatements(ifElement);
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
                // compile statments
                this.CompileStatements(ifElement);
                // compile closing curly brace
                this.CompileTerminal(ifElement);
            }
        }

        /// <summary>
        /// Compiles a while loop.
        /// </summary>
        private void CompileWhile(XElement parent)
        {
            XElement whileElement = new XElement("whileStatement");
            parent.Add(whileElement);

            // compile the while keyword
            this.CompileTerminal(whileElement);
            // compile the opening bracket 
            this.CompileTerminal(whileElement);
            //compile the expression
            this.CompileExpression(whileElement);
            //compile the closing bracket
            this.CompileTerminal(whileElement);
            //compile the opening curly bracket
            this.CompileTerminal(whileElement);
            //compile statements inside while 
            this.CompileStatements(whileElement);
            //compile the closing curly bracket
            this.CompileTerminal(whileElement);
        }

        private void CompileReturn(XElement parent)
        {
            XElement returnElement = new XElement("returnStatement");
            parent.Add(returnElement);

            // compile return keyword
            this.CompileTerminal(returnElement);

            // compile return expression
            if (this.classTokens.Peek().Value2 != ";")
            {
                this.CompileTokenIfExists(returnElement, "(");
                this.CompileExpression(returnElement);
                this.CompileTokenIfExists(returnElement, ")");
            }

            // compile the ;
            this.CompileTerminal(returnElement);
        }

        /// <summary>
        /// Compiles a terminal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private Pair<string,string> CompileTerminal(XElement parent)
        {
            Pair<string, string> terminal = null;
            if (this.classTokens.Count > 0)
            {
                terminal = this.classTokens.Pop();
                parent.Add(new XElement(terminal.Value1, terminal.Value2));
            }
            return terminal;
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
                this.CompileTerminal(expressionElement);
                this.CompileTerm(expressionElement);
            }
        }

        private void CompileTerm(XElement parent)
        {
            XElement termElement = new XElement("term");
            parent.Add(termElement);

            // compile the first part no matter what
            Pair<string, string> peekedToken = this.classTokens.Peek();
            this.CompileTerminal(termElement);
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
            }
            else if (this.IsSubRoutineCall())
            {
                this.CompileSubRoutineCall(termElement);
            }
        }

        private void CompileSubRoutineCall(XElement parent)
        {
            // subname'('expressionList')' 
            // OR
            // classOrVarName.subname'('expressionList')' 

            //subname or classOrVarName will have already been compiled

            if (this.classTokens.Peek().Value2 == "(")
            {
                // compile opening bracket
                this.CompileTerminal(parent);
                // compile the expression list
                this.CompileExpressionList(parent);
                // compile closing bracket
                this.CompileTerminal(parent);
            }
            else if (this.classTokens.Peek().Value2 == ".")
            {
                // compile the dot
                this.CompileTerminal(parent);
                // compile subName
                this.CompileTerminal(parent);
                // compile opening bracket
                this.CompileTerminal(parent);
                // compile the expression list                    
                this.CompileExpressionList(parent);
                // compile closing bracket
                this.CompileTerminal(parent);
            }
        }

        /// <summary>
        /// Compiles an expression list.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileExpressionList(XElement parent)
        {
            XElement expressionListElement = new XElement("expressionList");
            parent.Add(expressionListElement);

            // check for empty brackets
            if (this.classTokens.Peek().Value2 != ")")
            {
                this.CompileExpression(expressionListElement);

                while (this.classTokens.Peek().Value2 == ",")
                {
                    // comile comma
                    this.CompileTerminal(expressionListElement);

                    //compile expression
                    this.CompileExpression(expressionListElement);
                }
            }
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
            // TODO this is probably wrong - variable declarations etc are statments?!
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

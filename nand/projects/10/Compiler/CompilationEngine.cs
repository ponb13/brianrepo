﻿using System;
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
        /// Compile a class variable declaration
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        private void CompileClassVarDeclaration(XElement parentElement)
        {
            if (IsClassVariableDeclaration())
            {
                XElement classVariableElement = new XElement("classVarDec");
                parentElement.Add(classVariableElement);

                while (this.classTokens.Peek().Value2 != ";")
                {
                    Pair<string, string> token = this.classTokens.Pop();
                    classVariableElement.Add(new XElement(token.Value1, token.Value2));
                }

                // compile ;
                this.CompileTerminal(classVariableElement);

                // recursively handle all class variables
                this.CompileClassVarDeclaration(parentElement);

            }
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
        /// Compiles the variable declaration.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileVariableDeclaration(XElement parent)
        {
            if (this.classTokens.Peek().Value2 == "var")
            {
                XElement varDec = new XElement("varDec");
                parent.Add(varDec);

                // compile the var keyword
                this.CompileTerminal(varDec);
                // compile the type keyword
                this.CompileTerminal(varDec);
                // compile the identifier
                this.CompileTerminal(varDec);

                // check for comma
                if (this.classTokens.Peek().Value2 == ",")
                {
                    // compile the comma
                    this.CompileTerminal(varDec);

                    while (this.classTokens.Peek().Value2 != ";")
                    {
                        this.CompileTerminal(varDec);
                        if (this.classTokens.Peek().Value2 == ",")
                        {
                            this.CompileTerminal(varDec);
                        }
                    }
                }

                // compile the ending ;
                this.CompileTerminal(varDec);

                // recursively call variable declaration
                this.CompileVariableDeclaration(parent);
            }
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
                Pair<string, string> token = this.classTokens.Pop();
                parameterList.Add(new XElement(token.Value1, token.Value2));
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
            XElement doElement = new XElement("doStatement");
            parent.Add(doElement);

            // compile do keyword
            this.CompileTerminal(doElement);

            // this compile identifier, it can be in form of name.something.anotherThing(expression)  etc
            // so keep consuming until hit opening brace of expression
            while (this.classTokens.Peek().Value2 != "(")
            {
                this.CompileTerminal(doElement);
            }

            // compile opening brace of the expression
            this.CompileTerminal(doElement);
            // compile the expression
            this.CompileExpressionList(doElement);
            // compile clsoing brace of the expression
            this.CompileTerminal(doElement);
            // compile ;
            this.CompileTerminal(doElement);
        }

        /// <summary>
        /// Compiles a let statment.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileLet(XElement parent)
        {
            XElement letElement = new XElement("letStatement");
            parent.Add(letElement);

            // compile let keyword
            this.CompileTerminal(letElement);
            // compile identifier
            this.CompileTerminal(letElement);
            // compile =
            this.CompileTerminal(letElement);
            // compile expression
            this.CompileExpression(letElement);
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
                this.CompileExpression(returnElement);
            }

            // compile the ;
            this.CompileTerminal(returnElement);
        }

        /// <summary>
        /// Compiles a terminal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileTerminal(XElement parent)
        {
            if (this.classTokens.Count > 0)
            {
                Pair<string, string> terminal = this.classTokens.Pop();
                parent.Add(new XElement(terminal.Value1, terminal.Value2));
            }
        }

        /// <summary>
        /// Compiles an expression.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileExpression(XElement parent)
        {
            XElement expressionElement = new XElement("expression");
            parent.Add(expressionElement);

            XElement termElement = new XElement("term");
            parent.Add(termElement);

            this.CompileTerminal(expressionElement);

           
        }

        private void CompileTerm(XElement parent)
        {
            // BRIAN -Start here!!!
            // you have just writte is array accessor, now need to check if subroutine call see page 209
            // you have written some sub routine check ing code at bottom of class its commented out.
            
            XElement termElement = new XElement("term");
            parent.Add(termElement);
            
            Pair<string, string> peekedToken = this.classTokens.Peek();

            if (this.IsExpressionSimpleTerm())
            {
                this.CompileTerminal(termElement);
            }
            else if (peekedToken.Value2 == "(")
            {
                // if is expression compile it (compile the opening bracker first).
                this.CompileTerminal(termElement);
                this.CompileExpression(termElement);
            }
            else if (this.IsArrayAccessor())
            {

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

            // if not empty brackets
            while (this.classTokens.Peek().Value2 != ")")
            {
                this.CompileExpression(expressionListElement);
                //TODO must 

                //compile comma - comma separated expression list
                if (this.classTokens.Peek().Value2 == ",")
                {
                    this.CompileTerminal(expressionListElement);
                }
            }

        }

        private bool IsExpressionSimpleTerm()
        {
            Pair<string, string> token = this.classTokens.Peek();
            return ((token.Value1 == StringConstants.integerConstant) ||
                (token.Value1 == StringConstants.stringConstant) ||
                (token.Value1 == StringConstants.keyword) ||
                (token.Value1 == StringConstants.indentifier));
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

        /// <summary>
        /// Determines whether [is sub rourtine declaration].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is sub rourtine declaration]; otherwise, <c>false</c>.
        /// </returns>
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

        private bool IsArrayAccessor()
        {
            bool result = false;
            // need to read peek two deep into stack
            // pop one token off, peek the next one , and push popped one back
            Pair<string, string> poppedToken = this.classTokens.Pop();
            Pair<string, string> peekedToken = this.classTokens.Peek();

            if (poppedToken.Value1 == StringConstants.indentifier && peekedToken.Value1=="[")
            {
                result = true;
            }

            this.classTokens.Push(poppedToken);

            return result;
        }

        //private bool IsMethodCall()
        //{
        //    bool result = false;
        //    // need to read peek three deep into stack
        //    // pop two tokens off, peek the next one , and push popped two back
        //    Pair<string, string> poppedToken1 = this.classTokens.Pop();
        //    Pair<string, string> poppedToken2 = this.classTokens.Pop();
        //    Pair<string, string> peekedToken = this.classTokens.Peek();

        //    if (poppedToken.Value1 == StringConstants.indentifier && peekedToken.Value1 = "(")
        //    {
        //        result = true;
        //    }

        //    this.classTokens.Push(poppedToken2);
        //    this.classTokens.Push(poppedToken1);

        //    return result;
        //}

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
            // this is probably wrong - variable declarations etc are statments?!
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

        //private bool IsOperator()
        //{
        //    Pair<string, string> peekedToken = this.classTokens.Peek();
        //    Regex.Match(peekedToken.Value2, "[+|-|*|/|]", RegexOptions.Compiled);

        //    return false;
        //}
    }
}

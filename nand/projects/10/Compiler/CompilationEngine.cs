using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
            this.CompileTerm(classXml);
            // compiler class name
            this.CompileTerm(classXml);
            // compile opening curely of class
            this.CompileTerm(classXml);

            while (this.IsSubRourtineDeclaration())
            {
                
                this.CompileSubRoutine(classXml);
            }

            // compile class closing curely
            this.CompileTerm(classXml);


            return classXml;
        }

        /// <summary>
        /// Compile a class variable declaration
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        private void CompileClassVarDeclaration(XElement parentElement)
        {
            XElement classVariableElement = new XElement("classVarDec");
            parentElement.Add(classVariableElement);

            while (this.classTokens.Peek().Value2 != ";")
            {
                Pair<string, string> token = this.classTokens.Pop();
                classVariableElement.Add(new XElement(token.Value1, token.Value2));
            }

            this.CompileTerm(classVariableElement);

            if (IsClassVariableDeclaration())
            {
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
            this.CompileTerm(subRoutineElement);

            // add the param list
            this.CompileParameterList(subRoutineElement);

            // add closing bracket of params
            this.CompileTerm(subRoutineElement);

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
            this.CompileTerm(subRoutineBody);

            this.CompileVariableDeclaration(subRoutineBody);

            // while is statement || expression || variableDeclaration
            while (this.IsStatement() || this.IsVariableDeclaration()) // TODO or is expression or variable declaraiotn
            {
                if (this.IsStatement())
                {
                    // add statements to body
                    this.CompileStatements(subRoutineBody);
                }
                else if(this.IsVariableDeclaration())
                {
                    this.CompileVariableDeclaration(subRoutineBody);
                }
            }
            
            // add closing curely of methodBody
            this.CompileTerm(subRoutineBody);
        }

        /// <summary>
        /// Compiles the variable declaration.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileVariableDeclaration(XElement parent)
        {
            XElement varDec = new XElement("varDec");
            parent.Add(varDec);

            // compile the var keyword
            this.CompileTerm(varDec);
            // compile the type keyword
            this.CompileTerm(varDec);
            // compile the identifier
            this.CompileTerm(varDec);

            // check for comma
            if(this.classTokens.Peek().Value2 == ",")
            {
                // compile the comma
                this.CompileTerm(varDec);

                while (this.classTokens.Peek().Value2 != ";")
                {
                    this.CompileTerm(varDec);
                    if (this.classTokens.Peek().Value2 == ",")
                    {
                        this.CompileTerm(varDec);
                    }
                }
            }

            // compile the ending ;
            this.CompileTerm(varDec);

            
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
            if(this.IsStatement())
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
            this.CompileTerm(doElement);

            // this compile identifier, it can be in form of name.something.anotherThing(expression)  etc
            // so keep consuming until hit opening brace of expression
            while (this.classTokens.Peek().Value2 != "(")
            {
                this.CompileTerm(doElement);
            }

            // compile opening brace of the expression
            this.CompileTerm(doElement);
            // compile the expression
            this.CompileExpression(doElement);
            // compile clsoing brace of the expression
            this.CompileTerm(doElement);
            // compile ;
            this.CompileTerm(doElement);
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
            this.CompileTerm(ifElement);
            // compile  opening bracket
            this.CompileTerm(ifElement);
            // compiler expression
            this.CompileExpression(ifElement);
            // compile closing bracket
            this.CompileTerm(ifElement);
            // compile opening curly brace
            this.CompileTerm(ifElement);
            //compile the statement inside the if
            this.CompileStatements(ifElement);
            // compile closing curly brace
            this.CompileTerm(ifElement);

            // compile else statement if there is one
            if (this.classTokens.Peek().Value2 == "else")
            {
                // this maybe incorrect, may should create elseStatement and add children??
                // compile the else keyword
                this.CompileTerm(ifElement);
                // compile opening curly brace
                this.CompileTerm(ifElement);
                // compile statments
                this.CompileStatements(ifElement);
                // compile closing curly brace
                this.CompileTerm(ifElement);
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
            this.CompileTerm(whileElement);
            // compile the opening bracket 
            this.CompileTerm(whileElement);
            //compile the expression
            this.CompileExpression(whileElement);
            //compile the closing bracket
            this.CompileTerm(whileElement);
            //compile the opening curly bracket
            this.CompileTerm(whileElement);
            //compile statements inside while 
            this.CompileStatements(whileElement);
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
            this.CompileTerm(letElement);
            // compile identifier
            this.CompileTerm(letElement);
            // compile =
            this.CompileTerm(letElement);
            // compile expression
            this.CompileExpression(letElement);
            //compile ;
            this.CompileTerm(letElement);
        }

        private void CompileReturn(XElement parent)
        {
            XElement returnElement = new XElement("returnStatement");
            parent.Add(returnElement);

            // compile return keyword
            this.CompileTerm(returnElement);

            // compile return expression
            this.CompileExpression(returnElement);

            // compile the ;
            this.CompileTerm(returnElement);
        }

        /// <summary>
        /// Compiles a terminal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileTerm(XElement parent)
        {
            if (this.classTokens.Count > 0)
            {
                // TODO see page 216 for look ahead on array handling etc (peek!).
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
            // TODO this "term" element maybe it should be compile terminal????
            XElement expressionElement = new XElement("expression");
            parent.Add(expressionElement);
            XElement termElement = new XElement("term");
            expressionElement.Add(termElement);

            // ensure there is something between the brackets. i.e. not just closed brackets.
            if (this.classTokens.Peek().Value2 != ")")
            {
                Pair<string, string> expressionTermToken = this.classTokens.Pop();

                termElement.Add(new XElement(expressionTermToken.Value1, expressionTermToken.Value2));
            }


            // TODO - this is just a placeholder - will on compile expressionless...
            //this.CompileTerm(expressionElement);
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

        /// <summary>
        /// Determines whether the token is the beginning of a while statment
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// 	<c>true</c> if [is while statement] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsWhileStatement()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "while";
        }
    }
}

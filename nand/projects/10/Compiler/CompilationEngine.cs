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
        public CompilationEngine(IList<Pair<string,string>> classTokensList)
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

            if (this.IsSubRourtineDeclaration())
            {
                this.CompileSubRoutine(classXml);
            }

            // compile class closing curely
            this.CompileTerm(classXml);


            return classXml;
        }

        /// <summary>
        /// Compile a variable declaration
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
                Pair<string,string> token = this.classTokens.Pop();
                subRoutineElement.Add(new XElement(token.Value1, token.Value2));
            }

            // add the opening braket of param list
            this.CompileTerm(subRoutineElement);

            // add the param list
            this.CompileParameterList(subRoutineElement);

            // add closing bracket of params
            this.CompileTerm(subRoutineElement);

            // add opening curley of methodBody
            this.CompileTerm(subRoutineElement);

            // add statements to body
            this.CompileStatements(subRoutineElement);

            // add closing curely of methodBody
            this.CompileTerm(subRoutineElement);

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
            XElement statementsElement = new XElement("statements");
            parentElement.Add(statementsElement);
            
            if (this.IsWhileDeclaration())
            {
                this.CompileWhile(statementsElement);
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
        }

        /// <summary>
        /// Compiles a terminal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void CompileTerm(XElement parent)
        {
            // TODO see page 216 for look ahead on array handling etc (peek!).
            Pair<string, string> terminal = this.classTokens.Pop();
            parent.Add(new XElement(terminal.Value1, terminal.Value2));
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

            Pair<string, string> expressionTermToken = this.classTokens.Pop();

            termElement.Add(new XElement(expressionTermToken.Value1, expressionTermToken.Value2));


            // TODO - this is just a placeholder - will on compile expressionless...
            //this.CompileTerm(expressionElement);
        }

        /// <summary>
        /// Determines whether the token is the beginning of a while statment
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// 	<c>true</c> if [is while statement] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsWhileDeclaration()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && peekedToken.Value2 == "while";
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
            return peekedToken.Value1 == StringConstants.keyword && (peekedToken.Value2 == "field" || peekedToken.Value2 =="static");
        }

        /// <summary>
        /// Determines whether [is sub rourtine declaration].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is sub rourtine declaration]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSubRourtineDeclaration()
        {
            Pair<string, string> peekedToken = this.classTokens.Peek();
            return peekedToken.Value1 == StringConstants.keyword && (peekedToken.Value2 == "function" || peekedToken.Value2 == "constructor" || peekedToken.Value2 == "method");
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Interfaces;

namespace Compiler
{
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
            // TODO compile the class declaration tokens.
            Pair<string, string> classKeyword = this.classTokens.Pop();
            Pair<string, string> className = this.classTokens.Pop();
            Pair<string, string> classOpeningBrace = this.classTokens.Pop();

            XElement classXml = new XElement("class",
                                        new XElement(classKeyword.Value1, classKeyword.Value2),
                                        new XElement(className.Value1, className.Value2),
                                        new XElement(classOpeningBrace.Value1, classOpeningBrace.Value2));


            if (this.IsClassVariableDeclaration())
            {
                this.CompileClassVarDeclaration(classXml);
            }
            else if (this.IsSubRourtineDeclaration())
            {
                this.CompilerSubRoutine(classXml);
            }
            
            //this.CompileStatements(classXml);

            Pair<string, string> classClosingBrace = this.classTokens.Pop();
            classXml.Add(new XElement(classClosingBrace.Value1, classClosingBrace.Value2));


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

            Pair<string,string> semiColon = this.classTokens.Pop();
            classVariableElement.Add(new XElement(semiColon.Value1, semiColon.Value2));

            if (IsClassVariableDeclaration())
            {
                this.CompileClassVarDeclaration(parentElement);
            }
        }

        private void CompilerSubRoutine(XElement parentElement)
        {
            // start here brian!
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
            Pair<string,string> whileKeyword = this.classTokens.Pop();
            Pair<string,string> openingBracket = this.classTokens.Pop();
            Pair<string,string> expression = this.classTokens.Pop(); //TODO replace this with real expression parsing!
            Pair<string,string> closingBracket = this.classTokens.Pop();

            parent.Add(new XElement("whileStatement",
                    new XElement(whileKeyword.Value1, whileKeyword.Value2),
                    new XElement(openingBracket.Value1, openingBracket.Value2),
                    new XElement(expression.Value1, expression.Value2),
                    new XElement(closingBracket.Value1, closingBracket.Value2)));
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

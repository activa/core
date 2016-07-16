#region License
//=============================================================================
// VeloxDB Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2015 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Linq;

namespace Velox.Core.Parser
{
    public class ExpressionToken : Token
    {
        internal int NumTerms { get; set; }

        public new ExpressionTokenMatcher TokenMatcher { get { return (ExpressionTokenMatcher)base.TokenMatcher; } }

        public ExpressionToken()
        {
            Associativity = OperatorAssociativity.Left;
            throw new NotSupportedException();
        }

        protected ExpressionToken(string token) : base(null,token)
        {
            Associativity = OperatorAssociativity.Left;
        }

        public ExpressionToken(ExpressionTokenMatcher tokenMatcher, string text) : base(tokenMatcher, text)
        {
            switch (tokenMatcher.TokenType)
            {
                case TokenType.TernaryOperator: NumTerms = 3; break;
                case TokenType.UnaryOperator: NumTerms = 1; break;
                case TokenType.Operator: NumTerms = 2; break;
            }

            if (tokenMatcher.NumTerms != null)
                NumTerms = tokenMatcher.NumTerms.Value;

            Precedence = tokenMatcher.Precedence;
            Associativity = tokenMatcher.Associativity;
            TokenType = tokenMatcher.TokenType;
            Evaluator = tokenMatcher.Evaluator;
        }

        internal TokenType TokenType { get; set; }
        internal OperatorAssociativity Associativity { get; set; }
        internal int Precedence { get; set; }
        internal TokenEvaluator Evaluator { get; set; }

        internal bool IsOperator
        {
            get { return (TokenType == TokenType.Operator) || (TokenType == TokenType.UnaryOperator); }
        }

        internal bool IsTerm
        {
            get { return (TokenType == TokenType.Term); }
        }

        internal bool IsUnary
        {
            get { return (TokenType == TokenType.UnaryOperator); }
        }

        internal bool IsFunction
        {
            get { return (TokenType == TokenType.FunctionCall); }
        }

        internal bool IsLeftParen
        {
            get { return (TokenType == TokenType.LeftParen); }
        }

        internal bool IsRightParen
        {
            get { return (TokenType == TokenType.RightParen); }
        }

        internal bool IsArgumentSeparator
        {
            get { return TokenType == TokenType.ArgumentSeparator; }
        }

        public bool IsPartial
        {
            get { return TokenMatcher != null && TokenMatcher.IsPartial; }
        }

        public bool IsStatementSeperator
        {
            get { return TokenType == TokenType.StatementSeparator; }
        }

        public ExpressionToken Alternate
        {
            get
            {
                if (Alternates != null)
                    return (ExpressionToken) Alternates.FirstOrDefault();

                return null;
            }
        }

        public ExpressionTokenMatcher Root
        {
            get { return TokenMatcher.Root; }
        }

        public bool IsOpenBrace
        {
            get { return TokenType == TokenType.OpenBrace; }
        }

        public bool IsCloseBrace
        {
            get { return TokenType == TokenType.CloseBrace; }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
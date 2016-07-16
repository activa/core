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
using Velox.Core.Parser;

namespace Velox.Core.Json
{
    public class IntegerTokenMatcher : ITokenMatcher, ITokenProcessor
    {
        private bool _firstChar = true;

        public ITokenProcessor CreateTokenProcessor()
        {
            return new IntegerTokenMatcher();
        }

        public string TranslateToken(string originalToken, ITokenProcessor tokenProcessor)
        {
            return originalToken;
        }

        public void ResetState()
        {
            _firstChar = true;
        }

        public TokenizerState ProcessChar(char c, string fullExpression, int currentIndex)
        {
            try
            {
                if (c == '-' && _firstChar)
                    return TokenizerState.Valid;

                if (char.IsDigit(c))
                    return TokenizerState.Valid;

                return _firstChar ? TokenizerState.Fail : TokenizerState.Success;
            }
            finally
            {
                _firstChar = false;
            }
        }
    }

}
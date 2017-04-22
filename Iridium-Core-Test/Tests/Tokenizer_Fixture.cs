#region License
//=============================================================================
// Iridium-Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2017 Philippe Leybaert
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

using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class Tokenizer_Fixture
    {

        [Test]
        public void TestFallback()
        {
            ITokenMatcher matcher1 = new CharMatcher('(');
            ITokenMatcher matcher2 = new CharMatcher(')');
            ITokenMatcher matcher3 = new StringMatcher("(test)");
            ITokenMatcher matcher4 = new AnyCharMatcher("abcdefghijklmnopqrstuvwxyz");

            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(matcher1);
            tokenizer.AddTokenMatcher(matcher2);
            tokenizer.AddTokenMatcher(matcher3);
            tokenizer.AddTokenMatcher(matcher4);

            Token[] tokens = tokenizer.Tokenize("(test)(x)");

            Assert.AreEqual(4,tokens.Length);

        }

        [Test]
        public void TestAnySequence()
        {
            ITokenMatcher matcher = new CompositeMatcher(
                new AnyCharMatcher("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_"),
                new SequenceOfAnyCharMatcher("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789")
                );

            ITokenMatcher alphaMatcher = new SequenceOfAnyCharMatcher("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");

            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(matcher);
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());
            tokenizer.AddTokenMatcher(new CharMatcher('.'));

            Token[] tokens = tokenizer.Tokenize("Math.Max");

            Assert.AreEqual(3,tokens.Length);
            Assert.AreEqual("Math",tokens[0].Text);
            Assert.AreEqual(".", tokens[1].Text);
            Assert.AreEqual("Max", tokens[2].Text);




        }

        [Test]
        public void StartsAndEndsWithToken()
        {
            Tokenizer tokenizer = new Tokenizer();

            tokenizer.AddTokenMatcher(new StartsAndEndsWithMatcher("<!--","-->",'"'));
            tokenizer.AddTokenMatcher(new WhiteSpaceMatcher());

            Token[] tokens;

            tokens = tokenizer.Tokenize("<!--test-->  <!-- test 2 -->");

            Assert.AreEqual(3,tokens.Length);

            tokens = tokenizer.Tokenize("<!--test \"-->\"-->  <!-- test 2 -->");

            Assert.AreEqual(3, tokens.Length);
        }
    }
}
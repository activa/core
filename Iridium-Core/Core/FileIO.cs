#region License
//=============================================================================
// Iridium-Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2016 Philippe Leybaert
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
using System.IO;

namespace Iridium.Core
{
    public static class FileIO
    {
        private static IFileIOHandler _handler;

        private static IFileIOHandler Handler => _handler ?? (_handler = ServiceRepository.Default.Get<IFileIOHandler>());

        [Obsolete("Use ServiceRespository.Register(ioHandler)")]
        public static void SetIOHandler(IFileIOHandler handler)
        {
            ServiceRepository.Default.Register(handler);
        }

        public static string ReadAllText(string path)
        {
            return Handler.ReadAllText(path);
        }

        public static string[] ReadAllLines(string path)
        {
            return Handler.ReadAllLines(path);
        }

        public static void WriteAllText(string path, string s)
        {
            Handler.WriteAllText(path, s);
        }

        public static bool FileExists(string path)
        {
            return Handler.FileExists(path);
        }

        public static void Delete(string path)
        {
            Handler.Delete(path);
        }

        public static Stream OpenReadStream(string path, bool exclusive)
        {
            return Handler.OpenReadStream(path, exclusive);
        }

        public static Stream OpenWriteStream(string path, bool exclusive, bool create)
        {
            return Handler.OpenWriteStream(path, exclusive, create);
        }

        public static void AppendAllText(string path, string s)
        {
            Handler.AppendAllText(path, s);            
        }
    }
}

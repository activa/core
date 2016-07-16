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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Velox.Core
{
    public abstract class FileIOHandler
    {
        public abstract string ReadAllText(string path);
        public abstract string[] ReadAllLines(string path);
        public abstract void WriteAllText(string path, string s);
        public abstract bool FileExists(string path);
        public abstract void Delete(string path);
        public abstract Stream OpenReadStream(string path, bool exclusive);
        public abstract Stream OpenWriteStream(string path, bool exclusive, bool create);
        public abstract void AppendAllText(string path, string s);
    }

    public class DummyFileIOHandler : FileIOHandler
    {
        public override string ReadAllText(string path) { throw new NotSupportedException(); }
        public override string[] ReadAllLines(string path) { throw new NotSupportedException(); }
        public override void WriteAllText(string path, string s) { throw new NotSupportedException(); }
        public override bool FileExists(string path) { throw new NotSupportedException(); }
        public override void Delete(string path) { throw new NotSupportedException(); }
        public override Stream OpenReadStream(string path, bool exclusive) { throw new NotSupportedException(); }
        public override Stream OpenWriteStream(string path, bool exclusive, bool create) { throw new NotSupportedException(); }
        public override void AppendAllText(string path, string s) {  throw new NotSupportedException(); }
    }


    public static class FileIO
    {
        private static FileIOHandler _handler = new DummyFileIOHandler();

        public static void SetIOHandler(FileIOHandler handler)
        {
            _handler = handler;

        }

        public static string ReadAllText(string path)
        {
            return _handler.ReadAllText(path);
        }
        public static string[] ReadAllLines(string path)
        {
            return _handler.ReadAllLines(path);
        }
        public static void WriteAllText(string path, string s)
        {
            _handler.WriteAllText(path, s);
        }
        public static bool FileExists(string path)
        {
            return _handler.FileExists(path);
        }
        public static void Delete(string path)
        {
            _handler.Delete(path);
        }
        public static Stream OpenReadStream(string path, bool exclusive)
        {
            return _handler.OpenReadStream(path, exclusive);
        }
        public static Stream OpenWriteStream(string path, bool exclusive, bool create)
        {
            return _handler.OpenWriteStream(path, exclusive, create);
        }

        public static void AppendAllText(string path, string s)
        {
            _handler.AppendAllText(path, s);
            
        }
    }
}

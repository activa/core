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
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Core
{
    public class FileHistoryStore : IScheduleHistoryStore
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, DateTime> _lastRunTimes;
        private readonly string _fileName;

        public FileHistoryStore(string fileName)
        {
            _fileName = fileName;

			if (FileIO.FileExists(fileName))
            {
                try
                {
                    string[] lines = FileIO.ReadAllLines(fileName);

                    _lastRunTimes = (from line in lines
                                     let key = line.Split(' ')[0]
                                     let value = DateTime.FromBinary(long.Parse(line))
                                     select new { key,value}
                                         ).ToDictionary(item => item.key, item => item.value);

                    return;
                }
                catch // Some error when deserializing. Let's assume the data is invalid
                {
                }
            }
            
            _lastRunTimes = new Dictionary<string, DateTime>();
        }

        public DateTime LastRun(string taskId)
        {
            lock (_lock)
            {
                DateTime lastRun;

                return _lastRunTimes.TryGetValue(taskId, out lastRun) ? lastRun : DateTime.MinValue;
            }
        }

        public void SetLastRun(string taskId, DateTime lastRun)
        {
            lock (_lock)
            {
                _lastRunTimes[taskId] = lastRun;

				FileIO.WriteAllText(_fileName, string.Concat(from item in _lastRunTimes select item.Key + " " + item.Value.ToBinary() + "\r\n"));
            }
        }
    }
}
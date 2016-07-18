using System;
using System.Collections.Generic;
using System.Linq;

namespace Velox.Core.Scheduling
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
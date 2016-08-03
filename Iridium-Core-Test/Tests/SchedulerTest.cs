﻿#region License
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
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class SchedulerTest
    {
        MockTimeProvider _time = new MockTimeProvider();

        [Test]
        public void TestContinuous()
        {
            Scheduler scheduler = new CyclicScheduler(null, TimeSpan.FromMinutes(30));

            scheduler.TimeProvider = _time;

            Assert.IsTrue(scheduler.ShouldRun());
            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromMinutes(10);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromMinutes(20);

            Assert.IsTrue(scheduler.ShouldRun());
            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromMinutes(50);

            Assert.IsTrue(scheduler.ShouldRun());
            Assert.IsFalse(scheduler.ShouldRun());
        }

        [Test]
        public void TestContinuous2()
        {
            Scheduler scheduler1 = new CyclicScheduler(null, TimeSpan.FromMinutes(30));
            Scheduler scheduler2 = new CyclicScheduler(null, TimeSpan.FromMinutes(30));
            Scheduler scheduler3 = new CyclicScheduler(null, TimeSpan.FromMinutes(30));

            scheduler1.TimeProvider = _time;
            scheduler2.TimeProvider = _time;
            scheduler3.TimeProvider = _time;

            Assert.IsTrue(scheduler1.ShouldRun());
            Assert.IsFalse(scheduler1.ShouldRun());
            Assert.IsTrue(scheduler2.ShouldRun());
            Assert.IsFalse(scheduler2.ShouldRun());
            Assert.IsTrue(scheduler3.ShouldRun());
            Assert.IsFalse(scheduler3.ShouldRun());

            _time.Now += TimeSpan.FromMinutes(10);

            Assert.IsFalse(scheduler1.ShouldRun());
            Assert.IsFalse(scheduler2.ShouldRun());
            Assert.IsFalse(scheduler3.ShouldRun());

            _time.Now += TimeSpan.FromMinutes(20);

            Assert.IsTrue(scheduler1.ShouldRun());
            Assert.IsFalse(scheduler1.ShouldRun());
            Assert.IsTrue(scheduler2.ShouldRun());
            Assert.IsFalse(scheduler2.ShouldRun());
            Assert.IsTrue(scheduler3.ShouldRun());
            Assert.IsFalse(scheduler3.ShouldRun());

            _time.Now += TimeSpan.FromMinutes(50);

            Assert.IsTrue(scheduler1.ShouldRun());
            Assert.IsFalse(scheduler1.ShouldRun());
            Assert.IsTrue(scheduler2.ShouldRun());
            Assert.IsFalse(scheduler2.ShouldRun());
            Assert.IsTrue(scheduler3.ShouldRun());
            Assert.IsFalse(scheduler3.ShouldRun());
        }


        [Test]
        public void TestMonthly()
        {
            Scheduler scheduler = new MonthlyScheduler("MONTHLY", new TimeSpan(12, 0, 0), 5, 10);

            scheduler.TimeProvider = _time;

            _time.Now = new DateTime(2000, 1, 1);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(12);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(1);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now = new DateTime(2000, 1, 5, 11, 0, 0);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(1);

            Assert.IsTrue(scheduler.ShouldRun());
            Assert.IsFalse(scheduler.ShouldRun());

        }

        [Test]
        public void TestDaily()
        {
            Scheduler scheduler = new TimeOfDayScheduler("DAILY", new TimeSpan(12, 0, 0));

            scheduler.TimeProvider = _time;

            _time.Now = new DateTime(2000, 1, 1, 0, 0, 0);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(11);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(1);

            Assert.IsTrue(scheduler.ShouldRun());
            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(1);

            Assert.IsFalse(scheduler.ShouldRun());

            _time.Now += TimeSpan.FromHours(24);

            Assert.IsTrue(scheduler.ShouldRun());
            Assert.IsFalse(scheduler.ShouldRun());
        }

        [Test]
        public void TestFileHistoryStore()
        {
            string fileName = Path.Combine(Path.GetTempPath(), "__TEST__FILEHISTORY_STORE.TMP");

            if (File.Exists(fileName))
                File.Delete(fileName);

            FileHistoryStore historyStore = new FileHistoryStore(fileName);

            Assert.AreEqual(DateTime.MinValue,historyStore.LastRun("1"));

            DateTime ts1 = DateTime.Now;
            DateTime ts2 = ts1.AddDays(1);

            historyStore.SetLastRun("1",ts1);

            Assert.AreEqual(ts1, historyStore.LastRun("1"));

            historyStore.SetLastRun("2", ts2);

            Assert.AreEqual(ts1, historyStore.LastRun("1"));
            Assert.AreEqual(ts2, historyStore.LastRun("2"));

            File.Delete(fileName);
        }

        [Test]
        public void TestDefaultHistoryStore()
        {
            string id1 = Guid.NewGuid().ToString("N");
            string id2 = Guid.NewGuid().ToString("N");
            string id3 = Guid.NewGuid().ToString("N");

            DefaultHistoryStore historyStore = new DefaultHistoryStore();

            Assert.AreEqual(DateTime.MinValue, historyStore.LastRun(id1));
            Assert.AreEqual(DateTime.MinValue, historyStore.LastRun(id2));
            Assert.AreEqual(DateTime.MinValue, historyStore.LastRun(id3));

            DateTime ts1 = DateTime.Now;
            DateTime ts2 = ts1.AddDays(1);
            DateTime ts3 = ts1.AddDays(2);

            historyStore.SetLastRun(id1, ts1);
            historyStore.SetLastRun(id2, ts2);
            historyStore.SetLastRun(id3, ts3);

            Assert.AreEqual(ts1, historyStore.LastRun(id1));
            Assert.AreEqual(ts2, historyStore.LastRun(id2));
            Assert.AreEqual(ts3, historyStore.LastRun(id3));
        }

    }
}

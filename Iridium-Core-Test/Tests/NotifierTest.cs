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

using System.Linq;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class NotifierTest
    {
        private string _notificationResult;

        private Notifier CreateNotifierWithTypedSubscriptions()
        {
            _notificationResult = "";

            Notifier notifier = new Notifier();

            notifier.Subscribe(delegate(Notification<string> notification) { _notificationResult += notification.Payload + "A"; });
            notifier.Subscribe("x", delegate(Notification<string> notification) { _notificationResult += notification.Payload + "B"; });
            notifier.Subscribe("x|y", delegate(Notification<string> notification) { _notificationResult += notification.Payload + "C"; });
            notifier.Subscribe("y|z", delegate(Notification<string> notification) { _notificationResult += notification.Payload + "D"; });
            notifier.Subscribe(delegate(Notification<int> notification) { _notificationResult += "E"; });
            notifier.Subscribe("x", notification => _notificationResult += notification.Payload + "Z");

            return notifier;
        }

        [Test]
        public void TestTypedUnnamed()
        {
            Notifier notifier = CreateNotifierWithTypedSubscriptions();

            notifier.Post(null,"X");

            Assert.AreEqual("XA", _notificationResult);

            _notificationResult = "";

            notifier.Post(null,1);

            Assert.AreEqual("E", _notificationResult);
       }

        [Test]
        public void TestTypedNamed()
        {
            Notifier notifier = CreateNotifierWithTypedSubscriptions();

            notifier.Post("x","X");

            Assert.AreEqual("XAXBXCXZ", _notificationResult);

            _notificationResult = "";

            notifier.Post("y", "X");

            Assert.AreEqual("XAXCXD", _notificationResult);

            _notificationResult = "";

            notifier.Post("z", "X");

            Assert.AreEqual("XAXD", _notificationResult);
        }


        [Test]
        public void TestPassive()
        {
            Notifier notifier = new Notifier();

            ISubscription<int> subscription = notifier.Subscribe<int>(null);

            notifier.Post(null,1);
            notifier.Post(null,"x");

            var notifications = subscription.GetNotifications().ToList();

            Assert.AreEqual(1,notifications.Count);
            Assert.AreEqual(1,notifications[0].Payload);

            notifications = subscription.GetNotifications().ToList();

            Assert.AreEqual(0, notifications.Count);

        }

    }
}
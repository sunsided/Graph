using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Graph.Test
{
	[TestFixture]
	class LockTest
	{
		[Test]
		public void TestDelayed()
		{
			object lck = new object();
			Task task = new Task(() =>
			                     	{
										lock (lck)
										{
											Thread.Sleep(1000);
											Monitor.Pulse(lck);
										}
			                     	});
			lock(lck)
			{
				task.Start();
				Assert.IsTrue(Monitor.Wait(lck, 5000), "Mutex not released in time");
			}
		}
	}
}

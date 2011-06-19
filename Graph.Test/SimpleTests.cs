using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Graph.Test
{
	[TestFixture]
	public class SimpleTests
	{
		[Test]
		public void ChainTest()
		{
			ManualResetEvent waitHandle1 = new ManualResetEvent(false),
			                 waitHandle2 = new ManualResetEvent(false);

			// Teefilter
			IFilter<int, int> tee = new TeeFilter<int>();

			// Eine Senke definieren
			ISink<int> sink1 = new TerminatorSink<int>();
			sink1.StateChanged += (sender, args) => { if (args.State == ProcessState.Dispatching) Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Sink 1 - Wert erhalten: " + args.Input); waitHandle1.Set(); };

			// Quelle erzeugen, zwei Passthroughs und den Tee einhängen
			ISource<int> source = new ConstantSource<int>(10);
			source.Append(new PassthroughFilter<int>())
				.Append(new PassthroughFilter<int>())
				.Append(tee);

			// Debugging-Sink in den Tee einhängen
			tee.Append(new DelegateSink<int>(value => Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Tee - Wert erhalten: " + value)));

			// Threadwechsel, Delegate, 1-Sekunden-Delay und Sink
			tee.Append(new ThreadingFilter<int>())
				.Append(new DelegateFilter<int>((my, value) => value + ((int)my.Tag), 10))
				.Append(new DelegateFilter<int>(value => { Thread.Sleep(1000); return value; }))
				.Append(sink1);

			// Delegate und Sink (Hauptthread, unverzögert)
			tee.Append(new DelegateFilter<int, double>(value => value*0.75))
				.Append(new DelegateSink<double>((my, value) => { Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Sink 2 - Wert erhalten: " + value); ((ManualResetEvent) my.Tag).Set(); }, waitHandle2));
			
			// Und los!
			source.Process();
			Assert.IsTrue(waitHandle1.WaitOne(5000));
			Assert.IsTrue(waitHandle2.WaitOne(5000));
		}
	}
}

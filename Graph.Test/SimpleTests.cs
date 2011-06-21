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
			IFilter<double, double> tee2 = new TeeFilter<double>();

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
				.Append(new DelegateFilter<int>(value => { Thread.Sleep(2000); return value; }))
				.Append(new SetEventFilter<int>(waitHandle2))
				.Append(sink1);

			// Delegate und Sink (Hauptthread, unverzögert)
			tee.Append(new DelegateFilter<int, double>(value => value * 0.75))
				.Append(tee2)
				.Append(new DelegateFilter<double>(value => { Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Wert erhalten: " + value + " - warte auf Event ..."); return value; }))
				.Append(new WaitEventFilter<double>(waitHandle2))
				.Append(new DelegateSink<double>((my, value) => Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Sink 2 - Wert erhalten: " + value)));
			
			// Dingsi.
			tee2.Append(new DelegateFilter<double>(value => { Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Wert erhalten: " + value + " - erwartet."); return value; }));
			tee2.Append(new ConditionalPassthroughFilter<double>(value => value > 10))
				.Append(new DelegateFilter<double>(value => { Trace.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Wert erhalten: " + value + " - sollte nicht passieren."); return value; }));

			// Und los!
			source.Process();
			Assert.IsTrue(waitHandle1.WaitOne(10000));
		}
	}
}

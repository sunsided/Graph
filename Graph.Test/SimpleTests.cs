using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Graph.Test
{
	[TestFixture]
	public class SimpleTests
	{
		public static void Main(string[] args)
		{
			new SimpleTests().ChainTest();
		}

		[Test]
		public void ChainTest()
		{
			Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Verarbeitung begonnen.");

			const int count = 1;

			ManualResetEvent waitHandle1 = new ManualResetEvent(false),
			                 waitHandle = new ManualResetEvent(false);

			//Semaphore semaphore = new Semaphore(0, count);

			// Teefilter
			TeeFilter<int> tee = new TeeFilter<int>();
			TeeFilter<double> tee2 = new TeeFilter<double> {Tag = "Debug-Tee"};

			// Eine Senke definieren
			ISink<int> sink1 = new TerminatorSink<int>().MakeThreaded();
			sink1.StateChanged += (sender, args) => { if (args.State == ProcessState.Dispatching) Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Sink 1 - Wert erhalten: " + args.Input); waitHandle1.Set(); };

			// Quelle erzeugen, zwei Passthroughs und den Tee einhängen
			ISource<int> source = new ConstantSource<int>(10).MakeThreaded();
			source.Append(new PassthroughFilter<int>())
				.Append(new ActionFilter<int>(value => Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " -- Thread nach Start, Wert: " + value)))
				.Append(new PassthroughFilter<int>())
				.Append(tee);

			// Debugging-Sink in den Tee einhängen
			tee.Append(new ActionSink<int>(value => Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Tee - Wert erhalten: " + value)));

			// Threadwechsel, Delegate, 1-Sekunden-Delay und Sink
			tee.Append(new DelegateFilter<int>((my, value) => value + ((int)my.Tag), 10).MakeThreaded())
				.Append(new DelegateFilter<int>(value => { Thread.Sleep(2000); return value; }))
				.Append(new SetEventFilter<int>(waitHandle).AttachOutput(Console.Out, "Handle freigegeben"))
				.Append(sink1);

			// Delegate und Sink (Hauptthread, unverzögert)
			tee.Append(new DelegateFilter<int, double>(value => value * 0.75))
				.Append(tee2.MakeThreaded());

			// Dingsi.
			tee2.Append(new ActionFilter<double>(value => Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Wert erhalten: " + value + " - warte auf Event ...")))
				.Append(new WaitEventFilter<double>(waitHandle))
				.Append(new ActionSink<double>((my, value) => Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Sink 2 - Wert erhalten: " + value)));
			
			// Dingsi.
			tee2.Append(new DebuggerBreakFilter<double>())
				.Append(new ActionSink<double>(value => Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Wert erhalten: " + value + " - erwartet.")));

			// Dingsi.
			tee2.Append(new DebuggerBreakFilter<double>())
				.Append(new ConditionalPassthroughFilter<double>(value => value > 10))
				.Append(new ActionSink<double>(value => Console.WriteLine("#" + Thread.CurrentThread.ManagedThreadId + " Wert erhalten: " + value + " - sollte nicht passieren.")));

			// Und los!
			for (int i = 0; i < count; ++i)
			{
				source.Process();
			}
			Assert.IsTrue(waitHandle1.WaitOne(60000));
			Thread.Sleep(5000);
		}
	}
}

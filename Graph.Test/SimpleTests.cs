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

			ConstantSource<int> source = new ConstantSource<int>(10);
			PassthroughFilter<int> passthroughFilter1 = new PassthroughFilter<int>();
			PassthroughFilter<int> passthroughFilter2 = new PassthroughFilter<int>();
			TeeFilter<int> tee = new TeeFilter<int>();
			DelegateFilter<int> filter3 = new DelegateFilter<int>(value => value + 10);
			DelegateFilter<int, double> filter4 = new DelegateFilter<int, double>(value => value * 0.75);
			TerminatorSink<int> sink1 = new TerminatorSink<int>();
			TerminatorSink<double> sink2 = new TerminatorSink<double>();

			source.Append(passthroughFilter1)
				.Append(passthroughFilter2)
				.Append(tee);

			tee.Append(filter3)
				.Append(sink1);

			tee.Append(filter4)
				.Append(sink2);

			sink1.StateChanged += (sender, args) =>
			                      	{
			                      		if (args.State == ProcessState.Dispatching) Trace.WriteLine("Sink 1 - Wert erhalten: " + args.Input);
			                      		waitHandle1.Set();
			                      	};
			sink2.StateChanged += (sender, args) =>
			                      	{
			                      		if (args.State == ProcessState.Dispatching) Trace.WriteLine("Sink 2 - Wert erhalten: " + args.Input);
			                      		waitHandle2.Set();
			                      	};

			source.Process();
			Assert.IsTrue(waitHandle1.WaitOne(5000));
			Assert.IsTrue(waitHandle2.WaitOne(5000));
		}
	}
}

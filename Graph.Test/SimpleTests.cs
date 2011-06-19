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

			ISource<int> source = new ConstantSource<int>(10);
			IFilter<int, int> tee = new TeeFilter<int>();
			ISink<int> sink1 = new TerminatorSink<int>();
			ISink<double> sink2 = new DelegateSink<double>(value =>
			                                                      	{
			                                                      		Trace.WriteLine("Sink 2 - Wert erhalten: " + value);
			                                                      		waitHandle2.Set();
			                                                      	});

			source.Append(new PassthroughFilter<int>())
				.Append(new PassthroughFilter<int>())
				.Append(tee);

			tee.Append(new DelegateFilter<int>(value => value + 10))
				.Append(sink1);

			tee.Append(new DelegateFilter<int, double>(value => value * 0.75))
				.Append(sink2);

			sink1.StateChanged += (sender, args) =>
			                      	{
			                      		if (args.State == ProcessState.Dispatching) Trace.WriteLine("Sink 1 - Wert erhalten: " + args.Input);
			                      		waitHandle1.Set();
			                      	};

			source.Process();
			Assert.IsTrue(waitHandle1.WaitOne(5000));
			Assert.IsTrue(waitHandle2.WaitOne(5000));
		}
	}
}

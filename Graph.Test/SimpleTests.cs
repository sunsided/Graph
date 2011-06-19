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
			PassthroughFilter<int> passthroughFilter3 = new PassthroughFilter<int>();
			PassthroughFilter<int> passthroughFilter4 = new PassthroughFilter<int>();
			TerminatorSink<int> sink1 = new TerminatorSink<int>();
			TerminatorSink<int> sink2 = new TerminatorSink<int>();

			source.Append(passthroughFilter1)
				.Append(passthroughFilter2)
				.Append(tee);
			
			tee.Append(passthroughFilter3)
				.Append(sink1);

			tee.Append(passthroughFilter4)
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

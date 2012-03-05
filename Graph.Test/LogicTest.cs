using System.Threading;
using Graph.Logic;
using NUnit.Framework;

namespace Graph.Test
{
	/// <summary>
	/// Tests the logic classes
	/// </summary>
	[TestFixture]
	class LogicTest
	{
		/// <summary>
		/// Tests simple data dispatching
		/// </summary>
		[Test]
		public void TestDirect()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;
			LogicEmitter source = new LogicEmitter();
			source.AttachOutput(new LogicActionInvoker(value =>
			                                           	{
			                                           		result = value;
			                                           		autoResetEvent.Set();
			                                           	}));
			source.StartProcessing();

			Assert.IsFalse(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);

			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);

			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);

			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
		}

		/// <summary>
		/// Tests data negation
		/// </summary>
		[Test]
		public void TestNegation()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Create elements
			var source = new LogicEmitter();
			var filter = new NotGate();
			var sink = new LogicActionInvoker(value =>
			                                                 	{
			                                                 		result = value;
			                                                 		autoResetEvent.Set(); // wait for processing to finish
																});

			// Build the graph
			source.AttachOutput(filter);
			filter.AttachOutput(sink);
			
			// Start processing
			source.StartProcessing();

			Assert.IsFalse(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);

			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);

			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);

			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
		}

		/// <summary>
		/// Tests double negation of data
		/// </summary>
		[Test]
		public void TestDoubleNegation()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Create elements
			var source = new LogicEmitter();
			var filter1 = new NotGate();
			var filter2 = new NotGate();
		    var sink = new LogicActionInvoker(value =>
		                                          {
		                                              result = value;
		                                              autoResetEvent.Set();
		                                          });

			// Build the graph
			source.AttachOutput(filter1);
			filter1.AttachOutput(filter2);
			filter2.AttachOutput(sink);

			// Start processing
			source.StartProcessing();

			Assert.IsFalse(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);

			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);

			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);

			source.EmitFalse(); autoResetEvent.WaitOne();
			Assert.IsFalse(result);
			source.EmitTrue(); autoResetEvent.WaitOne();
			Assert.IsTrue(result);
		}

		/// <summary>
		/// Tests AND operation
		/// </summary>
		[Test]
		public void TestAnd()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Create the elements
			var source1 = new LogicEmitter();
			var source2 = new LogicEmitter();
			var gate = new AndGate();
		    var sink = new LogicActionInvoker(value =>
		                                          {
		                                              result = value;
		                                              autoResetEvent.Set();
		                                          });

			// Build the graph
			source1.AttachOutput(gate.Input1);
			source2.AttachOutput(gate.Input2);
			gate.AttachOutput(sink);

			// Start processing
			source1.StartProcessing();
			source2.StartProcessing();
			gate.StartProcessing();

			Assert.IsFalse(result);
			const int timeout = Timeout.Infinite;
			
			source1.EmitTrue(); source2.EmitTrue();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsTrue(result);

			source1.EmitTrue(); source2.EmitFalse();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsFalse(result);

			source1.EmitFalse(); source2.EmitTrue();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsFalse(result);

			source1.EmitFalse(); source2.EmitFalse();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsFalse(result);
		}

		/// <summary>
		/// Tests XNOR
		/// </summary>
		[Test]
		public void TestXnor()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Create elements
			var source1 = new LogicEmitter();
			var source2 = new LogicEmitter();
			var gate = new XnorGate();
		    var sink = new LogicActionInvoker(value =>
		                                          {
		                                              result = value;
		                                              autoResetEvent.Set();
		                                          });

			// Build the graph
			source1.AttachOutput(gate.Input1);
			source2.AttachOutput(gate.Input2);
			gate.AttachOutput(sink);

			// Start processing
			source1.StartProcessing();
			source2.StartProcessing();
			//gate.StartProcessing();

			Assert.IsFalse(result);
			const int timeout = Timeout.Infinite;

			source1.EmitTrue(); source2.EmitTrue();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsTrue(result);

			source1.EmitTrue(); source2.EmitFalse();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsFalse(result);

			source1.EmitFalse(); source2.EmitTrue();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsFalse(result);

			source1.EmitFalse(); source2.EmitFalse();
			Assert.IsTrue(autoResetEvent.WaitOne(timeout), "Timeout");
			Assert.IsTrue(result);
		}
	}
}

using System.Threading;
using Graph.Logic;
using NUnit.Framework;

namespace Graph.Test
{
	/// <summary>
	/// Tests der Logikklassen
	/// </summary>
	[TestFixture]
	class LogicTest
	{
		/// <summary>
		/// Testet den einfachen Durchgang von Daten
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
		/// Testet die Negation von Daten
		/// </summary>
		[Test]
		public void TestNegation()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Elemente erzeugen
			var source = new LogicEmitter();
			var filter = new NotGate();
			var sink = new LogicActionInvoker(value =>
			                                                 	{
			                                                 		result = value;
			                                                 		autoResetEvent.Set();
																});

			// Aufbauen
			source.AttachOutput(filter);
			filter.AttachOutput(sink);
			
			// Starten
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
		/// Testet die Negation von Daten
		/// </summary>
		[Test]
		public void TestDoubleNegation()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Elemente erzeugen
			var source = new LogicEmitter();
			var filter1 = new NotGate();
			var filter2 = new NotGate();
			var sink = new LogicActionInvoker(value =>
			{
				result = value;
				autoResetEvent.Set();
			});

			// Aufbauen
			source.AttachOutput(filter1);
			filter1.AttachOutput(filter2);
			filter2.AttachOutput(sink);

			// Starten
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
		/// Testet die Verundung von Daten
		/// </summary>
		[Test]
		public void TestAnd()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Elemente erzeugen
			var source1 = new LogicEmitter();
			var source2 = new LogicEmitter();
			var gate = new AndGate();
			var sink = new LogicActionInvoker(value =>
			{
				result = value;
				autoResetEvent.Set();
			});

			// Aufbauen
			source1.AttachOutput(gate.Input1);
			source2.AttachOutput(gate.Input2);
			gate.AttachOutput(sink);

			// Starten
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
		/// Testet die Ver-XNOR-ung von Daten
		/// </summary>
		[Test]
		public void TestXnor()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			bool result = false;

			// Elemente erzeugen
			var source1 = new LogicEmitter();
			var source2 = new LogicEmitter();
			var gate = new XnorGate();
			var sink = new LogicActionInvoker(value =>
			{
				result = value;
				autoResetEvent.Set();
			});

			// Aufbauen
			source1.AttachOutput(gate.Input1);
			source2.AttachOutput(gate.Input2);
			gate.AttachOutput(sink);

			// Starten
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

using System.Threading;
using Graph.Filters.LogicGates;
using Xunit;

namespace Graph.Test
{
    /// <summary>
    /// Tests the logic classes
    /// </summary>
    public sealed class LogicTest
    {
        /// <summary>
        /// Tests simple data dispatching
        /// </summary>
        [Fact]
        public void TestDirect()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var result = false;

            // Create elements / build the graph
            var source = new LogicEmitter();
            source.AttachOutput(new LogicActionInvoker(value =>
            {
                result = value;
                autoResetEvent.Set();
            }));

            // Start processing
            source.StartProcessing();

            // Test initial state
            Assert.False(result);

            // Emit an test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);
        }

        /// <summary>
        /// Tests data negation
        /// </summary>
        [Fact]
        public void TestNegation()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var result = false;

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

            // Test initial state
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.False(result);
        }

        /// <summary>
        /// Tests double negation of data
        /// </summary>
        [Fact]
        public void TestDoubleNegation()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var result = false;

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

            // Test initial state
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitFalse();
            autoResetEvent.WaitOne();
            Assert.False(result);

            // Emit and test
            source.EmitTrue();
            autoResetEvent.WaitOne();
            Assert.True(result);
        }

        /// <summary>
        /// Tests AND operation
        /// </summary>
        [Fact]
        public void TestAnd()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var result = false;

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

            // Test initial state
            Assert.False(result);
            const int timeout = Timeout.Infinite;

            // Emit and test
            source1.EmitTrue();
            source2.EmitTrue();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.True(result);

            // Emit and test
            source1.EmitTrue();
            source2.EmitFalse();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.False(result);

            // Emit and test
            source1.EmitFalse();
            source2.EmitTrue();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.False(result);

            // Emit and test
            source1.EmitFalse();
            source2.EmitFalse();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.False(result);
        }

        /// <summary>
        /// Tests XNOR
        /// </summary>
        [Fact]
        public void TestXnor()
        {
            var autoResetEvent = new AutoResetEvent(false);
            var result = false;

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

            // Test initial state
            Assert.False(result);
            const int timeout = Timeout.Infinite;

            // Emit and test
            source1.EmitTrue();
            source2.EmitTrue();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.True(result);

            // Emit and test
            source1.EmitTrue();
            source2.EmitFalse();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.False(result);

            // Emit and test
            source1.EmitFalse();
            source2.EmitTrue();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.False(result);

            // Emit and test
            source1.EmitFalse();
            source2.EmitFalse();
            Assert.True(autoResetEvent.WaitOne(timeout), "Timeout");
            Assert.True(result);
        }
    }
}

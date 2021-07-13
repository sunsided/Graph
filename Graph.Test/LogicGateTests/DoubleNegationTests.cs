using System;
using System.Threading;
using FluentAssertions;
using Graph.Filters.LogicGates;
using Xunit;

namespace Graph.Test.LogicGateTests
{
    public sealed class DoubleNegationTests : IDisposable
    {
        private readonly LogicEmitter _source;
        private readonly NotGate _filter1;
        private readonly NotGate _filter2;
        private readonly LogicActionInvoker _sink;
        private readonly AutoResetEvent _autoResetEvent = new(false);
        private bool _result;

        public DoubleNegationTests()
        {
            _source = new LogicEmitter();
            _filter1 = new NotGate();
            _filter2 = new NotGate();
            _sink = new LogicActionInvoker(value =>
            {
                _result = value;
                _autoResetEvent.Set();
            });

            _source.AttachOutput(_filter1);
            _filter1.AttachOutput(_filter2);
            _filter2.AttachOutput(_sink);

            _source.StartProcessing();
        }

        [Fact]
        public void NumberOfOutputProcessors_IsOne()
        {
            _source.OutputProcessorCount.Should().Be(1);
            _filter1.OutputProcessorCount.Should().Be(1);
            _filter2.OutputProcessorCount.Should().Be(1);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _source.StopProcessing();
            _source.Dispose();
            _filter1.Dispose();
            _filter2.Dispose();
            _sink.Dispose();
            _autoResetEvent?.Dispose();
        }

        [Fact]
        public void InitialState_IsFalse()
        {
            _result.Should().BeFalse();
        }

        [Fact]
        public void Values_AreNotNegated()
        {
            var testSequence = new[] {true, true, false, false, true, false, false, true};
            foreach (var value in testSequence)
            {
                _source.Emit(value);
                _autoResetEvent.WaitOne();
                _result.Should().Be(value);
            }
        }
    }
}

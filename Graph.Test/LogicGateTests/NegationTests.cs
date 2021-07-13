using System;
using System.Threading;
using FluentAssertions;
using Graph.Filters.LogicGates;
using Xunit;

namespace Graph.Test.LogicGateTests
{
    public sealed class NegationTests : IDisposable
    {
        private readonly LogicEmitter _source;
        private readonly NotGate _filter;
        private readonly LogicActionInvoker _sink;
        private readonly AutoResetEvent _autoResetEvent = new(false);
        private bool _result;

        public NegationTests()
        {
            _source = new LogicEmitter();
            _filter = new NotGate();
            _sink = new LogicActionInvoker(value =>
            {
                _result = value;
                _autoResetEvent.Set();
            });

            _source.AttachOutput(_filter);
            _filter.AttachOutput(_sink);

            _source.StartProcessing();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _source.StopProcessing();
            _source.Dispose();
            _filter.Dispose();
            _sink.Dispose();
            _autoResetEvent?.Dispose();
        }

        [Fact]
        public void NumberOfOutputProcessors_IsOne()
        {
            _source.OutputProcessorCount.Should().Be(1);
            _filter.OutputProcessorCount.Should().Be(1);
        }

        [Fact]
        public void InitialState_IsFalse()
        {
            _result.Should().BeFalse();
        }

        [Fact]
        public void Values_AreNegated()
        {
            var testSequence = new[] {true, true, false, false, true, false, false, true};
            foreach (var value in testSequence)
            {
                _source.Emit(value);
                _autoResetEvent.WaitOne();
                _result.Should().Be(!value);
            }
        }
    }
}

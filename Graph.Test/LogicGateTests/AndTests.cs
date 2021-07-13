using System;
using System.Threading;
using FluentAssertions;
using Graph.Filters.LogicGates;
using Xunit;

namespace Graph.Test.LogicGateTests
{
    public sealed class AndTests : IDisposable
    {
        private readonly LogicEmitter _source1;
        private readonly LogicEmitter _source2;
        private readonly AndGate _filter;
        private readonly LogicActionInvoker _sink;
        private readonly AutoResetEvent _autoResetEvent = new(false);
        private bool _result;

        public AndTests()
        {
            _source1 = new LogicEmitter();
            _source2 = new LogicEmitter();
            _filter = new AndGate();
            _sink = new LogicActionInvoker(value =>
            {
                _result = value;
                _autoResetEvent.Set();
            });

            _source1.AttachOutput(_filter.Input1);
            _source2.AttachOutput(_filter.Input2);
            _filter.AttachOutput(_sink);

            _source1.StartProcessing();
            _source2.StartProcessing();
            _filter.StartProcessing();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _source1.StopProcessing();
            _source2.StopProcessing();
            _filter.StopProcessing();
            _source1.Dispose();
            _source2.Dispose();
            _filter.Dispose();
            _sink.Dispose();
            _autoResetEvent?.Dispose();
        }

        [Fact]
        public void NumberOfOutputProcessors_IsOne()
        {
            _source1.OutputProcessorCount.Should().Be(1);
            _source2.OutputProcessorCount.Should().Be(1);
            _filter.OutputProcessorCount.Should().Be(1);
        }

        [Fact]
        public void InitialState_IsFalse()
        {
            _result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public void Values_AreCombined(bool a, bool b, bool expected)
        {
            _source1.Emit(a);
            _source2.Emit(b);
            _autoResetEvent.WaitOne(Timeout.Infinite).Should().BeTrue();
            _result.Should().Be(expected);
        }
    }
}

using System;
using System.Threading;
using FluentAssertions;
using Graph.Filters.LogicGates;
using Xunit;

namespace Graph.Test.LogicGateTests
{
    public sealed class InputToOutputTests : IDisposable
    {
        private readonly LogicEmitter _source;
        private readonly AutoResetEvent _autoResetEvent = new(false);
        private bool _result;

        public InputToOutputTests()
        {
            _source = new LogicEmitter();
            _source.AttachOutput(new LogicActionInvoker(value =>
            {
                _result = value;
                _autoResetEvent.Set();
            }));

            _source.StartProcessing();
        }

        [Fact]
        public void NumberOfOutputProcessors_IsOne()
        {
            _source.OutputProcessorCount.Should().Be(1);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _source.StopProcessing();
            _source.Dispose();
            _autoResetEvent?.Dispose();
        }

        [Fact]
        public void InitialState_IsFalse()
        {
            _result.Should().BeFalse();
        }

        [Fact]
        public void Values_ArePassedThrough()
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

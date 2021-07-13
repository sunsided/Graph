using System.Collections.Generic;
using System.Threading;
using Graph.Sources;

namespace Graph.Filters.LogicGates
{
    /// <summary>
    /// Source that emits boolean values.
    /// </summary>
    public sealed class LogicEmitter : DataSource<bool>
    {
        /// <summary>
        /// WaitHandle to control the value creation.
        /// </summary>
        private readonly AutoResetEvent _starter = new(false);

        /// <summary>
        /// Timeout in milliseconds.
        /// </summary>
        private const int StarterTimeoutMs = 1000;

        /// <summary>
        /// Value emission queue.
        /// </summary>
        private readonly Queue<bool> _emissionQueue = new();

        /// <summary>
        /// Creates the data.
        /// </summary>
        /// <param name="payload">The created data</param>
        /// <returns>
        /// <see cref="DataSource{TData}.SourceResult.Process"/> if the processing should continue; <see cref="DataSource{TData}.SourceResult.StopProcessing"/> if the output (<paramref name="payload"/>)
        /// should be discarded and the processing stopped; <see cref="DataSource{TData}.SourceResult.Idle"/> if nothing should happen.
        /// </returns>
        protected override SourceResult CreateData(out bool payload)
        {
            lock (_emissionQueue)
            {
                if (_emissionQueue.Count > 0)
                {
                    payload = _emissionQueue.Dequeue();
                    return SourceResult.Process;
                }
            }

            _starter.WaitOne(StarterTimeoutMs);
            payload = false; // Don't care
            return SourceResult.Idle;
        }

        /// <summary>
        /// Emits the value <see langword="true" />.
        /// <para>A call to <see cref="DataSource{T}.StartProcessing"/> is needed to start processing!</para>
        /// </summary>
        public void EmitTrue()
        {
            Emit(true);
        }

        /// <summary>
        /// Emits the value <see langword="false" />.
        /// <para>A call to <see cref="DataSource{T}.StartProcessing"/> is needed to start processing!</para>
        /// </summary>
        public void EmitFalse()
        {
            Emit(false);
        }

        /// <summary>
        /// Emits the given value
        /// <para>A call to <see cref="DataSource{T}.StartProcessing"/> is needed to start processing!</para>
        /// </summary>
        /// <param name="value">The value to emit</param>
        public void Emit(bool value)
        {
            lock (_emissionQueue)
            {
                _emissionQueue.Enqueue(value);
                _starter.Set();
            }
        }
    }
}

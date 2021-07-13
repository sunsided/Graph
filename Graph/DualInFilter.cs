using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
    /// <summary>
    /// Data filter with two inputs.
    /// </summary>
    /// <typeparam name="TInput1">First input data type</typeparam>
    /// <typeparam name="TInput2">Second input data type</typeparam>
    /// <typeparam name="TOutput">Output data type</typeparam>
    public abstract class DualInFilter<TInput1, TInput2, TOutput> : DataSource<TOutput>, IDualInFilter<TInput1, TInput2, TOutput>
    {
        /// <summary>
        /// Sink that passes data to the parent queue.
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        private sealed class PassthroughSink<T> : DataSink<T>
        {
            /// <summary>
            /// The linked queue.
            /// </summary>
            private Queue<T> Queue { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;.PassthroughSink&lt;T&gt;"/> class.
            /// </summary>
            /// <param name="queue">The queue.</param>
            /// <remarks></remarks>
            public PassthroughSink(Queue<T> queue)
            {
                Queue = queue;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;.PassthroughSink&lt;T&gt;"/> class.
            /// </summary>
            /// <param name="queue">The queue.</param>
            /// <param name="registrationTimeout">Timeout in milliseconds to be used during value registration.</param>
            /// <param name="inputQueueLength">Maximum queue length for input values.</param>
            /// <remarks></remarks>
            public PassthroughSink(Queue<T> queue, [DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
                : base(registrationTimeout, inputQueueLength)
            {
                Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
                Contract.Requires(inputQueueLength > 0);
                Queue = queue;
            }

            /// <summary>
            /// Enqueues the data
            /// </summary>
            /// <param name="payload">The data to enqueue</param>
            protected override void ProcessData(T payload)
            {
                // no locking needed here as the base classes already does this
                Queue.Enqueue(payload);
            }
        }

        /// <summary>
        /// Default value for locking timeouts.
        /// </summary>
        internal const int RegistrationTimeoutDefault = Timeout.Infinite;

        /// <summary>
        /// Default value for the input queue size.
        /// </summary>
        internal const int InputQueueLengthDefault = 100;

        /// <summary>
        /// First input.
        /// </summary>
        private readonly Queue<TInput1> _input1 = new Queue<TInput1>();

        /// <summary>
        /// Second input.
        /// </summary>
        private readonly Queue<TInput2> _input2 = new Queue<TInput2>();

        /// <summary>
        /// Backing field for the first input
        /// </summary>
        private readonly PassthroughSink<TInput1> _inputSink1;

        /// <summary>
        /// Backing field for the second input
        /// </summary>
        private readonly PassthroughSink<TInput2> _inputSink2;

        /// <summary>
        /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
        /// </summary>
        protected DualInFilter()
        {
            Contract.Ensures(_inputSink1 != null && _inputSink2 != null);

            _inputSink1 = new PassthroughSink<TInput1>(_input1);
            _inputSink2 = new PassthroughSink<TInput2>(_input2);
            StartProcessing();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
        /// </summary>
        /// <param name="outputQueueLength">Length of the output queue.</param>
        protected DualInFilter([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
            : base(outputQueueLength)
        {
            Contract.Requires(outputQueueLength > 0);
            Contract.Ensures(_inputSink1 != null && _inputSink2 != null);
            Contract.Ensures(Input1 == _inputSink1);
            Contract.Ensures(Input2 == _inputSink2);

            _inputSink1 = new PassthroughSink<TInput1>(_input1);
            _inputSink2 = new PassthroughSink<TInput2>(_input2);
            StartProcessing();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
        /// </summary>
        /// <param name="outputQueueLength">Length of the output queue.</param>
        /// <param name="registrationTimeout">Der Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten werden soll.</param>
        /// <param name="inputQueueLength">Die maximale Anzahl an Elementen in der Eingangsqueue.</param>
        protected DualInFilter([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength, [DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
            : base(outputQueueLength)
        {
            Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
            Contract.Requires(inputQueueLength > 0);
            Contract.Requires(outputQueueLength > 0);
            Contract.Ensures(_inputSink1 != null && _inputSink2 != null);
            Contract.Ensures(Input1 == _inputSink1);
            Contract.Ensures(Input2 == _inputSink2);

            _inputSink1 = new PassthroughSink<TInput1>(_input1, registrationTimeout, inputQueueLength);
            _inputSink2 = new PassthroughSink<TInput2>(_input2, registrationTimeout, inputQueueLength);
            StartProcessing();
        }

        /// <inheritdoc />
        public ISink<TInput1> Input1
        {
            [Pure] get
            {
                Contract.Ensures(Contract.Result<ISink<TInput1>>() != null);
                return _inputSink1;
            }
        }

        /// <inheritdoc />
        public ISink<TInput2> Input2
        {
            [Pure] get
            {
                Contract.Ensures(Contract.Result<ISink<TInput1>>() != null);
                return _inputSink2;
            }
        }

        /// <inheritdoc />
        public sealed override void StartProcessing()
        {
            _inputSink1.StartProcessing();
            _inputSink2.StartProcessing();
            base.StartProcessing();
        }

        /// <inheritdoc />
        public override void StopProcessing()
        {
            _inputSink1.StopProcessing();
            _inputSink2.StopProcessing();
            base.StopProcessing();
        }

        /// <summary>
        /// Produces the output data
        /// </summary>
        /// <param name="payload">The generated data</param>
        /// <returns>
        /// <see cref="DataSource{TData}.SourceResult.Process"/> if the processing should continue; <see cref="DataSource{TData}.SourceResult.StopProcessing"/> if the output
        /// (<paramref name="payload"/>) should be discarded and the processing should be stopped; <see cref="DataSource{TData}.SourceResult.Idle"/> if nothing should happen.
        /// </returns>
        protected override SourceResult CreateData(out TOutput payload)
        {
            payload = default(TOutput);
            if (_input1.Count == 0) return SourceResult.Idle;
            if (_input2.Count == 0) return SourceResult.Idle;

            TInput1 value1 = _input1.Dequeue();
            TInput2 value2 = _input2.Dequeue();

            // Process the data.
            TOutput output;
            if (ProcessData(value1, value2, out output))
            {
                payload = output;
                return SourceResult.Process;
            }

            // Do nothing.
            payload = default(TOutput);
            return SourceResult.Idle;
        }

        /// <summary>
        /// Processes the data
        /// </summary>
        /// <param name="input1">The first input data type</param>
        /// <param name="input2">The second input data type</param>
        /// <param name="output">The output data type</param>
        /// <returns>
        /// <see langword="true" /> if the result should be send to the outputs; <see langword="false" /> if the result should be discarded.
        /// </returns>
        protected abstract bool ProcessData(TInput1 input1, TInput2 input2, out TOutput output);

        /// <summary>
        /// Contract invariant.
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_inputSink1 != null);
            Contract.Invariant(_inputSink2 != null);
        }
    }

    /// <summary>
    /// Filter with two identical inputs.
    /// </summary>
    /// <typeparam name="TInput">The input data type</typeparam>
    /// <typeparam name="TOutput">The output data type</typeparam>
    public abstract class DualInFilter<TInput, TOutput> : DualInFilter<TInput, TInput, TOutput>, IDualInFilter<TInput, TOutput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
        /// </summary>
        protected DualInFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
        /// </summary>
        /// <param name="outputQueueLength">Length of the output queue.</param>
        protected DualInFilter([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
            : base(outputQueueLength)
        {
            Contract.Requires(outputQueueLength > 0);
        }
    }
}

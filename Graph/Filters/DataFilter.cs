﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;
using Graph.Processors;
using Graph.Sinks;

namespace Graph.Filters
{
    /// <summary>
    /// Base class for a data processor with outputs.
    /// </summary>
    public abstract class DataFilter<TInput, TOutput> : DataProcessor<TInput>, IFilter<TInput, TOutput>
    {
        /// <summary>
        /// The list of attached output data processors.
        /// </summary>
        private readonly List<IDataInput<TOutput>> _outputList = new();

        /// <summary>
        /// The count of attached output data processors.
        /// </summary>
        public int OutputProcessorCount
        {
            [Pure]
            get
            {
                lock (_outputList)
                {
                    return _outputList.Count;
                }
            }
        }

        /// <summary>
        /// The queue of output data processors to dispatch to.
        /// </summary>
        private readonly Queue<IDataInput<TOutput>> _currentOutputs = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFilter&lt;TInput, TOutput&gt;"/> class.
        /// </summary>
        protected DataFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFilter&lt;TInput, TOutput&gt;"/> class.
        /// </summary>
        /// <param name="registrationTimeout">Timeout in milliseconds to be used during value registration.</param>
        /// <param name="inputQueueLength">Maximum queue length for input values.</param>
        protected DataFilter([DefaultValue(RegistrationTimeoutDefault)]
            int registrationTimeout, [DefaultValue(InputQueueLengthDefault)]
            int inputQueueLength)
            : base(registrationTimeout, inputQueueLength)
        {
            Contract.Requires(registrationTimeout is Timeout.Infinite or > 0);
            Contract.Requires(inputQueueLength > 0);
        }

        /// <summary>
        /// Registers a processor for the output values.
        /// </summary>
        /// <param name="outputProcessor">The processor to register.</param>
        /// <returns><see langword="true" /> if the processor was registered successfully; <see langword="false" /> otherwise</returns>
        public bool AttachOutput(IDataInput<TOutput> outputProcessor)
        {
            //Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_outputList.Count) + 1 == _outputList.Count) ||
            //                  (!Contract.Result<bool>() && Contract.OldValue(_outputList.Count) == _outputList.Count));

            // Register and start processing
            lock (_outputList)
            {
                if (_outputList.Contains(outputProcessor)) return false;
                _outputList.Add(outputProcessor);
                return true;
            }
        }

        /// <summary>
        /// Unregisters a processor.
        /// </summary>
        /// <param name="outputProcessor">The processor to unregister.</param>
        /// <returns><see langword="true" /> if the processor was removed successfully; <see langword="false" /> otherwise</returns>
        public bool DetachOutput(IDataInput<TOutput> outputProcessor)
        {
            //Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_outputList.Count) - 1 == _outputList.Count) ||
            //                  (!Contract.Result<bool>() && Contract.OldValue(_outputList.Count) == _outputList.Count));

            // Kick it
            lock (_outputList)
            {
                return _outputList.Remove(outputProcessor);
            }
        }

        /// <summary>
        /// Processes the data.
        /// </summary>
        /// <param name="payload">The data to process.</param>
        protected sealed override void ProcessData(TInput payload)
        {
            OnProcessingStateChanged(ProcessingState.Processing);
            var dispatch = ProcessData(payload, out var outputPayload);

            // Dispatch
            if (!dispatch) return;
            lock (_outputList)
            {
                OnProcessingStateChanged(ProcessingState.Dispatching);
                // Enqueue processors
                _currentOutputs.Clear();
                _outputList.ForEach(processor => _currentOutputs.Enqueue(processor));
            }

            // Loop until all processors have been fed
            while (_currentOutputs.Count > 0)
            {
                var processor = _currentOutputs.Dequeue();
                if (!processor.RegisterInput(outputPayload))
                {
                    _currentOutputs.Enqueue(processor);
                }
            }
        }

        /// <summary>
        /// Processes the data.
        /// </summary>
        /// <param name="input">The input data type</param>
        /// <param name="output">The output data type</param>
        /// <returns>
        /// <see langword="true" /> if the result should be handed to the outputs; <see langword="false" />
        /// if the result should be discarded.
        /// </returns>
        protected abstract bool ProcessData(TInput input, out TOutput output);
    }
}

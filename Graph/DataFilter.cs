using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
	/// <summary>
	/// Base class for a data processor with output
	/// </summary>
	public abstract class DataFilter<TInput, TOutput> : DataProcessor<TInput>, IFilter<TInput, TOutput>
	{
		/// <summary>
        /// The list of attached output data processors
		/// </summary>
		private readonly List<IDataInput<TOutput>> _outputList = new List<IDataInput<TOutput>>();

		/// <summary>
		/// The count of attached output data processors
		/// </summary>
		public int OutputProcessorCount { [Pure] get { return _outputList.Count; } }

		/// <summary>
		/// The queue of output data processors to dispatch to
		/// </summary>
		private readonly Queue<IDataInput<TOutput>> _currentOutputs = new Queue<IDataInput<TOutput>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFilter&lt;TInput, TOutput&gt;"/> class.
        /// </summary>
        /// <remarks></remarks>
		protected DataFilter()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFilter&lt;TInput, TOutput&gt;"/> class.
        /// </summary>
        /// <param name="registrationTimeout">Timeout in milliseconds to be used during value registration.</param>
        /// <param name="inputQueueLength">Maximum queue length for input values.</param>
        /// <remarks></remarks>
		protected DataFilter([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
			: base(registrationTimeout, inputQueueLength)
		{
			Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
			Contract.Requires(inputQueueLength > 0);
		}

		/// <summary>
		/// Registers a processor for the output values
		/// </summary>
		/// <param name="outputProcessor">The processor to register.</param>
		/// <returns><c>true</c> if the processor was registered successfully; <c>false</c> otherwise</returns>
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
		/// Unregisters a processor
		/// </summary>
		/// <param name="outputProcessor">The processor to unregister.</param>
		/// <returns><c>true</c> if the processor was removed successfully; <c>false</c> otherwise</returns>
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
		/// Processes the data
		/// </summary>
		/// <param name="payload">Die zu verarbeitenden Daten</param>
		protected sealed override void ProcessData(TInput payload)
		{
			TOutput outputPayload;
			OnProcessingStateChanged(ProcessingState.Processing);
			bool dispatch = ProcessData(payload, out outputPayload);

			// Dispatch
			if (!dispatch) return;
			lock(_outputList)
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
		/// Processes the data
		/// </summary>
		/// <param name="input">The input data type</param>
		/// <param name="output">The output data type</param>
		/// <returns>
		/// <c>true</c> if the result should be handed to the outputs; <c>false</c>
		/// if the result should be discarded.
		/// </returns>
		protected abstract bool ProcessData(TInput input, out TOutput output);
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Graph
{
    /// <summary>
    /// Base class for data processors with output
    /// </summary>
    public abstract class DataSource<TData> : DataProcessorBase, ISource<TData>, IDisposable
    {
        /// <summary>
        /// Processing state
        /// </summary>
        public enum SourceResult
        {
            /// <summary>
            /// Do nothing
            /// </summary>
            Idle,

            /// <summary>
            /// Process data
            /// </summary>
            Process,

            /// <summary>
            /// Stop processing
            /// </summary>
            StopProcessing
        }

        /// <summary>
        /// The list of output processors
        /// </summary>
        private readonly List<IDataInput<TData>> _outputList = new List<IDataInput<TData>>();

        /// <summary>
        /// Gets the output processor count.
        /// </summary>
        public int OutputProcessorCount { [Pure] get { return _outputList.Count; } }

        /// <summary>
        /// The list of outputs to dispatch data to.
        /// </summary>
        private readonly Queue<IDataInput<TData>> _currentOutputs = new Queue<IDataInput<TData>>();

        /// <summary>
        /// The size of the output queue
        /// </summary>
        private readonly Queue<TData> _outputQueue = new Queue<TData>();

        /// <summary>
        /// The processing task
        /// </summary>
        private readonly Task _processingTask;

        /// <summary>
        /// The output task
        /// </summary>
        private readonly Task _outputTask;

        /// <summary>
        /// Determines if the processing should stop
        /// </summary>
        private volatile bool _stopProcessing;

        /// <summary>
        /// Default value for <see cref="OutputQueueLength"/>
        /// </summary>
        internal const int OutputQueueLengthDefault = 100;

        /// <summary>
        /// The semaphore to control access to the output queue
        /// <seealso cref="_outputQueue"/>
        /// </summary>
        private readonly Semaphore _outputQueueSemaphore;

        /// <summary>
        /// The maximum size of the output queue
        /// </summary>
        public int OutputQueueLength { [Pure] get; private set; }

        /// <summary>
        /// Thread synchronization object that controls the output loop
        /// </summary>
        private readonly AutoResetEvent _outputStartTrigger = new AutoResetEvent(false);

        /// <summary>
        /// Creates a new instance of the <seealso cref="DataSource{TData}"/> class
        /// </summary>
        protected DataSource()
            : this(OutputQueueLengthDefault)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource&lt;TData&gt;"/> class.
        /// </summary>
        /// <param name="outputQueueLength">Length of the output queue.</param>
        protected DataSource([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
        {
            Contract.Requires(outputQueueLength > 0);

            OutputQueueLength = outputQueueLength;
            _outputQueueSemaphore = new Semaphore(outputQueueLength, outputQueueLength);

            _processingTask = new Task(ProcessingLoop, TaskCreationOptions.LongRunning);
            _outputTask = new Task(OutputLoop, TaskCreationOptions.LongRunning);

            // TODO: Automatically start the sink?
            //_processingTask.Start(); // TODO: customizable Scheduler?
            //_outputTask.Start(); // TODO: customizable scheduler?
        }

        /// <summary>
        /// Registers a processor of the output values
        /// </summary>
        /// <param name="outputProcessor">The processor to register.</param>
        /// <returns><see langword="true" /> if the processor was added successfully; <see langword="false" /> otherwise</returns>
        public bool AttachOutput(IDataInput<TData> outputProcessor)
        {
            //Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_outputList.Count) + 1 == _outputList.Count) ||
            //                  (!Contract.Result<bool>() && Contract.OldValue(_outputList.Count) == _outputList.Count));

            // Enqueue element and start processing.
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
        /// <returns><see langword="true" /> if the processor was successfully removed; <see langword="false" /> otherwise</returns>
        public bool DetachOutput(IDataInput<TData> outputProcessor)
        {
            //Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_outputList.Count) - 1 == _outputList.Count) ||
            //                  (!Contract.Result<bool>() && Contract.OldValue(_outputList.Count) == _outputList.Count));

            // Enqueue element and start processing.
            lock (_outputList)
            {
                return _outputList.Remove(outputProcessor);
            }
        }

        /// <inheritdoc />
        public override void StartProcessing()
        {
            if (_processingTask.Status == TaskStatus.WaitingToRun || _processingTask.Status == TaskStatus.Running) return;
            _processingTask.Start();
            _outputTask.Start();
        }

        /// <inheritdoc />
        public override void StopProcessing()
        {
            Contract.Ensures(_stopProcessing == true);
            _stopProcessing = true;
            _outputStartTrigger.Set();
            OnProcessingStateChanged(ProcessingState.Stopped);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            StopProcessing();
            Task.WhenAll(_outputTask, _processingTask).Wait();

            _outputStartTrigger.Dispose();
            _outputQueueSemaphore.Dispose();
            _outputTask.Dispose();
            _processingTask.Dispose();
        }

        /// <summary>
        /// Creates the data
        /// </summary>
        /// <param name="payload">The created data</param>
        /// <returns>
        /// <see cref="SourceResult.Process"/> if the processing should continue; <see cref="SourceResult.StopProcessing"/> if the output (<paramref name="payload"/>)
        /// should be discarded and the processing stopped; <see cref="SourceResult.Idle"/> if nothing should happen.
        /// </returns>
        protected abstract SourceResult CreateData(out TData payload);

        /// <summary>
        /// Produces data in an infinite loop
        /// <seealso cref="StartProcessing"/>
        /// <seealso cref="StopProcessing"/>
        /// </summary>
        private void ProcessingLoop()
        {
            do
            {
                var result = CreateData(out var payload);
                if (result == SourceResult.Idle)
                {
                    Thread.Sleep(0);
                    if (_stopProcessing) break;
                    continue;
                }
                if(result == SourceResult.StopProcessing)
                {
                    break;
                }

                // Enqueue the payload.
                _outputQueueSemaphore.WaitOne(); // TODO: Timeout!
                _outputQueue.Enqueue(payload);
                _outputStartTrigger.Set();

            } while (!_stopProcessing);
        }

        /// <summary>
        /// The output loop.
        /// </summary>
        private void OutputLoop()
        {
            while (!_stopProcessing)
            {
                _outputStartTrigger.WaitOne();
                OnProcessingStateChanged(ProcessingState.Dispatching);

                // Dispatch while data available
                int count;
                lock (_outputQueue) count = _outputQueue.Count;
                while (!_stopProcessing && count-- > 0)
                {
                    // Get result value
                    TData outputPayload;
                    lock (_outputQueue)
                    {
                        outputPayload = _outputQueue.Dequeue();
                        _outputQueueSemaphore.Release(1);
                    }

                    // Get outputs
                    lock (_outputList)
                    {
                        _currentOutputs.Clear();
                        _outputList.ForEach(processor => _currentOutputs.Enqueue(processor));
                    }

                    // Process all outputs.
                    while (_currentOutputs.Count > 0)
                    {
                        var processor = _currentOutputs.Dequeue();
                        if (!processor.RegisterInput(outputPayload))
                        {
                            _currentOutputs.Enqueue(processor);
                        }
                    }
                }
            }
        }
    }
}

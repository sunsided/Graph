using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Graph
{
    /// <summary>
    /// Base class for data processors
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <remarks></remarks>
    public abstract class DataProcessor<TData> : DataProcessorBase, IDataInput<TData>, IDisposable
    {
        /// <summary>
        /// Default value for <see cref="_registrationTimeout"/>.
        /// </summary>
        internal const int RegistrationTimeoutDefault = Timeout.Infinite;

        /// <summary>
        /// Default value for <see cref="InputQueueLength"/>
        /// </summary>
        internal const int InputQueueLengthDefault = 100;

        /// <summary>
        /// Timeout in milliseconds to be used during data registration.
        /// </summary>
        /// <seealso cref="RegisterInput"/>
        private readonly int _registrationTimeout;

        /// <summary>
        /// Timeout in milliseconds while waiting for registered elements.
        /// </summary>
        /// <seealso cref="RegisterInput"/>
        private const int ProcessingLockTimeout = Timeout.Infinite;

        /// <summary>
        /// The input value queue
        /// </summary>
        private readonly Queue<TData> _inputQueue = new Queue<TData>();

        /// <summary>
        /// The semaphore to control the input queue
        /// </summary>
        /// <seealso cref="RegisterInput"/>
        /// <seealso cref="_inputQueue"/>
        private readonly Semaphore _inputQueueSemaphore;

        /// <summary>
        /// The maximum size of the input queue
        /// </summary>
        public int InputQueueLength { [Pure] get; private set; }

        /// <summary>
        /// Thread sync object to control the processing loop
        /// </summary>
        private readonly AutoResetEvent _processStartTrigger = new AutoResetEvent(false);

        /// <summary>
        /// Determines if the processing loop should stop
        /// </summary>
        private volatile bool _stopProcessing;

        /// <summary>
        /// The processing task
        /// </summary>
        private readonly Task _processingTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessor{T}"/> class.
        /// </summary>
        internal DataProcessor()
            : this(RegistrationTimeoutDefault, InputQueueLengthDefault)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessor{T}"/> class.
        /// </summary>
        /// <param name="registrationTimeout">Timeout in milliseconds to be used during value registration.</param>
        /// <param name="inputQueueLength">Maximum queue length for input values.</param>
        internal DataProcessor([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
        {
            Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
            Contract.Requires(inputQueueLength > 0);

            _registrationTimeout = registrationTimeout;
            InputQueueLength = inputQueueLength;
            _inputQueueSemaphore = new Semaphore(inputQueueLength, inputQueueLength);

            _processingTask = new Task(ProcessingLoop, TaskCreationOptions.LongRunning);
            _processingTask.Start(); // TODO: Scheduler wählbar?
        }

        /// <summary>
        /// Registers an input value
        /// <para>
        /// If the input queue has free slots, this call is non-blocking, otherwise it is blocked
        /// until a queue slot is freed.
        /// </para>
        /// </summary>
        /// <param name="input">The value to register.</param>
        public bool RegisterInput(TData input)
        {
            // Warten, bis ein Eingabeslot frei wird
            if (!_inputQueueSemaphore.WaitOne(_registrationTimeout)) return false;

            // Element eintüten und Verarbeitung starten lassen
            lock (_inputQueue) _inputQueue.Enqueue(input);
            _processStartTrigger.Set();
            return true;
        }

        /// <summary>
        /// Processes the input data until <see cref="_stopProcessing"/> is set to <c>true</c>.
        /// <para>
        /// If there is no input in the input queue, the execution blocks until <see cref="_processStartTrigger"/> is set.
        /// </para>
        /// </summary>
        /// <seealso cref="RegisterInput"/>
        /// <seealso cref="StopProcessing"/>
        private void ProcessingLoop()
        {
            Queue<TData> processingQueue = new Queue<TData>();
            while (!_stopProcessing)
            {
                // Wait for the start signal
                OnProcessingStateChanged(ProcessingState.Idle);
                if (!_processStartTrigger.WaitOne(ProcessingLockTimeout)) continue;

                // Get the data
                lock (_inputQueue)
                {
                    if (_inputQueue.Count == 0) continue;
                    OnProcessingStateChanged(ProcessingState.Preparing);
                    int count = _inputQueue.Count;
                    while (count-- > 0)
                    {
                        processingQueue.Enqueue(_inputQueue.Dequeue());
                        _inputQueueSemaphore.Release(1);
                    }
                }

                // Process the data
                while(processingQueue.Count > 0)
                {
                    TData payload = processingQueue.Dequeue();
                    ProcessDataInternal(payload);
                }
            }
        }

        /// <summary>
        /// Stops the processing
        /// </summary>
        public override void StopProcessing()
        {
            Contract.Ensures(_stopProcessing == true);
            _stopProcessing = true;
            _processStartTrigger.Set();
            OnProcessingStateChanged(ProcessingState.Stopped);
        }

        /// <summary>
        /// Starts the processing
        /// </summary>
        public override void StartProcessing()
        {
            if (_processingTask.Status == TaskStatus.WaitingToRun || _processingTask.Status == TaskStatus.Running) return;
            _processingTask.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            StopProcessing();
            _processingTask.Wait();

            _inputQueueSemaphore.Dispose();
            _processingTask.Dispose();
            _processStartTrigger.Dispose();
        }

        /// <summary>
        /// Processes a data element
        /// </summary>
        /// <param name="payload">The data to process</param>
        protected abstract void ProcessData(TData payload);

        /// <summary>
        /// Processes a data element
        /// </summary>
        /// <param name="payload">The data to process</param>
        private void ProcessDataInternal(TData payload)
        {
            try
            {
                OnProcessingStateChanged(ProcessingState.Processing);
                ProcessData(payload);
            }
            catch(Exception e)
            {
                OnExceptionCaught(e);
            }
        }
    }
}

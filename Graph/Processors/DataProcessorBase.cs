using System;
using System.Diagnostics.Contracts;

namespace Graph.Processors
{
    /// <summary>
    /// Base for data processors.
    /// </summary>
    public abstract class DataProcessorBase : IDataProcessor, IDisposable
    {
        /// <summary>
        /// Gets or sets a user defined tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag
        {
            [Pure]
            get;
            set;
        }

        /// <summary>
        /// Gets the current processing state
        /// </summary>
        public ProcessingState State
        {
            [Pure]
            get;
            private set;
        }

        /// <summary>
        /// Occurs when the processing state (<see cref="DataProcessorBase.State"/>) changes.
        /// </summary>
        public event EventHandler<ProcessingStateEventArgs> ProcessingStateChanged;

        /// <summary>
        /// Occurs when an exception was caught.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionCaught;

        /// <summary>
        /// Starts the processing.
        /// </summary>
        public abstract void StartProcessing();

        /// <summary>
        /// Stops the processing.
        /// </summary>
        public abstract void StopProcessing();

        /// <summary>
        /// Internal constructor to prevent construction outside this assembly.
        /// </summary>
        internal DataProcessorBase()
        {
        }

        ~DataProcessorBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Raises the <see cref="ProcessingStateChanged"/> event.
        /// </summary>
        /// <param name="state">The <see cref="ProcessingState"/>.</param>
        protected virtual void OnProcessingStateChanged(ProcessingState state)
        {
            if (state == State) return;
            State = state;
            var handler = ProcessingStateChanged;
            handler?.Invoke(this, new ProcessingStateEventArgs(state));
        }

        /// <summary>
        /// Invokes the exception caught.
        /// </summary>
        /// <param name="e">The <see cref="Graph.ExceptionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExceptionCaught(Exception e)
        {
            var handler = ExceptionCaught;
            handler?.Invoke(this, new ExceptionEventArgs(e));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="IDisposable" />
        /// <param name="disposing">Whether this instance is currently disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            StopProcessing();
        }
    }
}

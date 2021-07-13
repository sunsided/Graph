using System;
using System.Diagnostics.Contracts;

namespace Graph
{
    /// <summary>
    /// Base for data processors
    /// </summary>
    public abstract class DataProcessorBase : IDataProcessor
    {
        /// <summary>
        /// Gets or sets a user defined tag.
        /// </summary>
        /// <value>The tag.</value>
        /// <remarks></remarks>
        public object Tag { [Pure] get; set; }

        /// <summary>
        /// Gets the current processing state
        /// </summary>
        /// <remarks></remarks>
        public ProcessingState State { [Pure] get; private set; }

        /// <summary>
        /// Occurs when the processing state (<see cref="DataProcessorBase.State"/>) changes.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<ProcessingStateEventArgs> ProcessingStateChanged;

        /// <summary>
        /// Occurs when an exception was caught
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<ExceptionEventArgs> ExceptionCaught;

        /// <summary>
        /// Beginnt die Verarbeitung
        /// </summary>
        public abstract void StartProcessing();

        /// <summary>
        /// Stops the processing
        /// </summary>
        /// <remarks></remarks>
        public abstract void StopProcessing();

        /// <summary>
        /// Internal constructor to prevent construction outside this assembly
        /// </summary>
        internal DataProcessorBase()
        {
        }

        /// <summary>
        /// Raises the <see cref="ProcessingStateChanged"/> event.
        /// </summary>
        /// <param name="state">The <see cref="ProcessingState"/>.</param>
        /// <remarks></remarks>
        protected virtual void OnProcessingStateChanged(ProcessingState state)
        {
            if (state == State) return;
            EventHandler<ProcessingStateEventArgs> handler = ProcessingStateChanged;
            if (handler != null) handler(this, new ProcessingStateEventArgs(state));
        }

        /// <summary>
        /// Invokes the exception caught.
        /// </summary>
        /// <param name="e">The <see cref="Graph.ExceptionEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        protected virtual void OnExceptionCaught(Exception e)
        {
            EventHandler<ExceptionEventArgs> handler = ExceptionCaught;
            if (handler != null) handler(this, new ExceptionEventArgs(e));
        }
    }
}

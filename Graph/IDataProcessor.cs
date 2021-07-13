using System;
using System.Diagnostics.Contracts;

namespace Graph
{
    /// <summary>
    /// Interface for data processors.
    /// </summary>
    public interface IDataProcessor
    {
        /// <summary>
        /// Gets or sets a user defined tag.
        /// </summary>
        /// <value>The tag.</value>
        object Tag { [Pure] get; set; }

        /// <summary>
        /// Gets the current processing state.
        /// </summary>
        ProcessingState State { [Pure] get; }

        /// <summary>
        /// Occurs when the processing state (<see cref="DataProcessorBase.State"/>) changes.
        /// </summary>
        event EventHandler<ProcessingStateEventArgs> ProcessingStateChanged;

        /// <summary>
        /// Occurs when an exception was caught.
        /// </summary>
        event EventHandler<ExceptionEventArgs> ExceptionCaught;

        /// <summary>
        /// Starts the processing.
        /// </summary>
        void StartProcessing();

        /// <summary>
        /// Stops the processing.
        /// </summary>
        void StopProcessing();
    }
}

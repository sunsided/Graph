using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Graph
{
    /// <summary>
    /// Event arguments for exceptions.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Exception) + "}")]
    public sealed class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { [Pure] get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingStateEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Graph.Processors
{
    /// <summary>
    /// Event arguments for processing states.
    /// </summary>
    [DebuggerDisplay("{" + nameof(State) + "}")]
    public sealed class ProcessingStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingStateEventArgs"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public ProcessingStateEventArgs(ProcessingState state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the processing state.
        /// </summary>
        public ProcessingState State { [Pure] get; private init; }
    }
}

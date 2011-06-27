using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Exception-Ereignisparameter
	/// </summary>
	[DebuggerDisplay("{Exception}")]
	public sealed class ExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Die Exception
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

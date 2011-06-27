using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Verarbeitungszustand-Ereignisparameter
	/// </summary>
	[DebuggerDisplay("{State}")]
	public sealed class ProcessingStateEventArgs : EventArgs
	{
		/// <summary>
		/// Der Verarbeitungszustand
		/// </summary>
		public ProcessingState State { [Pure] get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessingStateEventArgs"/> class.
		/// </summary>
		/// <param name="state">The state.</param>
		public ProcessingStateEventArgs(ProcessingState state)
		{
			State = state;
		}
	}
}

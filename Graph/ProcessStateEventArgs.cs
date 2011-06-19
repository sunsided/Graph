using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graph
{
	/// <summary>
	/// <see cref="EventArgs"/>, die einen Prozess-Zustandswechsel beschreiben
	/// </summary>
	public class ProcessStateEventArgs : EventArgs
	{
		/// <summary>
		/// Der Zustand des Prozessors
		/// </summary>
		public ProcessState State { get; private set; }

		/// <summary>
		/// Die verarbeitenden Daten, wenn vorhanden
		/// </summary>
		public object Input { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessStateEventArgs"/> class.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="input">The input.</param>
		public ProcessStateEventArgs(ProcessState state, object input)
		{
			State = state;
			Input = input;
		}
	}
}

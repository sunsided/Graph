using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Basisklasse für Elemente mit State
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StateBase : IProcessIndicator
	{
		/// <summary>
		/// Objekt für Threadsynchronisierung
		/// </summary>
		protected readonly object SyncRoot = new object();

		/// <summary>
		/// Der Prozesszustand hat sich geändert
		/// </summary>
		public event EventHandler<ProcessStateEventArgs> StateChanged;

		/// <summary>
		/// Setzt den Bearbeitungsstatus
		/// </summary>
		/// <param name="state">Der Zustand, in dem sich das Element befindet</param>
		/// <param name="currentInput">Der derzeitige Input, falls vorhanden</param>
		protected void SetProcessingState(ProcessState state, object currentInput)
		{
			if (StateChanged != null) StateChanged(this, new ProcessStateEventArgs(state, currentInput));
		}
	}
}
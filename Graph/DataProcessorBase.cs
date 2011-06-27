using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Basisklasse für ein Datenverarbeitungselement
	/// </summary>
	public abstract class DataProcessorBase : IDataProcessor
	{
		/// <summary>
		/// Liest oder setzt das benutzerdefinierte Tag
		/// </summary>
		public object Tag { [Pure] get; set; }

		/// <summary>
		/// Ermittelt den aktuellen Bearbeitungsstand
		/// </summary>
		public ProcessingState State { [Pure] get; private set; }

		/// <summary>
		/// Wird gerufen, wenn sich der Verarbeitungszustand (<see cref="State"/>) geändert hat
		/// </summary>
		public event EventHandler<ProcessingStateEventArgs> ProcessingStateChanged;

		/// <summary>
		/// Wird gerufen, wenn eine Exception aufgetreten ist
		/// </summary>
		public event EventHandler<ExceptionEventArgs> ExceptionCaught;

		/// <summary>
		/// Beginnt die Verarbeitung
		/// </summary>
		public abstract void StartProcessing();

		/// <summary>
		/// Hält die Verarbeitung an
		/// </summary>
		public abstract void StopProcessing();

		/// <summary>
		/// Interner Konstruktor, um Instanzierung außerhalb dieser Assembly zu verhindern
		/// </summary>
		internal DataProcessorBase() {}

		/// <summary>
		/// Raises the <see cref="ProcessingStateChanged"/> event.
		/// </summary>
		/// <param name="state">The <see cref="ProcessingState"/>.</param>
		/// <remarks></remarks>
		protected virtual void OnProcessingStateChanged(ProcessingState state)
		{
			Contract.Ensures(State != state);
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
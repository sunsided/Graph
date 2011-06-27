using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	public interface IDataProcessor
	{
		/// <summary>
		/// Liest oder setzt das benutzerdefinierte Tag
		/// </summary>
		object Tag { [Pure] get; set; }

		/// <summary>
		/// Ermittelt den aktuellen Bearbeitungsstand
		/// </summary>
		ProcessingState State { [Pure] get; }

		/// <summary>
		/// Wird gerufen, wenn sich der Verarbeitungszustand (<see cref="DataProcessorBase.State"/>) geändert hat
		/// </summary>
		event EventHandler<ProcessingStateEventArgs> ProcessingStateChanged;

		/// <summary>
		/// Wird gerufen, wenn eine Exception aufgetreten ist
		/// </summary>
		event EventHandler<ExceptionEventArgs> ExceptionCaught;

		/// <summary>
		/// Beginnt die Verarbeitung
		/// </summary>
		void StartProcessing();

		/// <summary>
		/// Hält die Verarbeitung an
		/// </summary>
		void StopProcessing();
	}
}
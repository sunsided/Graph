using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Interface für Elemente, die Daten einlesen
	/// </summary>
	/// <typeparam name="TData">Der Eingangsdatentyp</typeparam>
	public interface IDataInput<in TData> : IDataProcessor
	{
		/// <summary>
		/// Die Länge der Eingabequeue
		/// </summary>
		int InputQueueLength { [Pure] get; }

		/// <summary>
		/// Registriert einen Eingabewert
		/// <para>
		/// Wenn die Eingabequeue frei ist, ist dieser Aufruf nicht blockierend. 
		/// Ist die Queue voll, blockiert der Aufruf so lange, bis neue Werte 
		/// nachgereicht werden können.
		/// </para>
		/// </summary>
		/// <param name="input">Der zu registrierende Eingabewert.</param>
		bool RegisterInput(TData input);
	}
}
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Interface f�r Elemente, die Daten ausgeben
	/// </summary>
	/// <typeparam name="TOutput">Der Ausgangsdatentyp des Elementes. Entspricht Eingangsdatenyp des angeschlossenen <see cref="IDataInput{TOutput}"/>.</typeparam>
	public interface IDataOutput<out TOutput>
	{
		/// <summary>
		/// Die Anzahl der Ausgabeprozessoren
		/// </summary>
		int OutputProcessorCount { [Pure] get; }

		/// <summary>
		/// Registriert einen Prozessor f�r die Ausgabewerte
		/// </summary>
		/// <param name="outputProcessor">Der zu registrierende Prozessor.</param>
		/// <returns><c>true</c>, wenn der Prozessor erfolgreich hinzugef�gt wurde, ansonsten <c>false</c></returns>
		bool AttachOutput(IDataInput<TOutput> outputProcessor);

		/// <summary>
		/// Registriert einen Prozessor f�r die Ausgabewerte
		/// </summary>
		/// <param name="outputProcessor">Der zu registrierende Prozessor.</param>
		/// <returns><c>true</c>, wenn der Prozessor erfolgreich hinzugef�gt wurde, ansonsten <c>false</c></returns>
		bool DetachOutput(IDataInput<TOutput> outputProcessor);
	}
}
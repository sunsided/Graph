using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Senke, die Daten verwirft
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	public class TerminatorSink<TIn> : ISink<TIn>
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		public void Process(TIn input)
		{
		}

		/// <summary>
		/// Ermittelt, ob das Objekt gerade beschäftigt ist
		/// </summary>
		public ProcessState State
		{
			[Pure] get { return ProcessState.Idle; }
		}
	}
}

namespace Graph.Sinks
{
	/// <summary>
	/// Senke, die Daten verwirft
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	public class TerminatorSink<TIn> : StateBase, ISink<TIn>
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		public void Process(TIn input)
		{
			SetProcessingState(ProcessState.Dispatching, input);
			SetProcessingState(ProcessState.Idle, null);
		}
	}
}

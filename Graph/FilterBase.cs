namespace Graph
{
	/// <summary>
	/// Basisklasse für Filter
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	/// <typeparam name="TOut">Ausgabeparameter</typeparam>
	public abstract class FilterBase<TIn, TOut> : ElementBase<TOut>, IFilter<TIn, TOut>
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		public abstract TOut Filter(TIn input);

		/// <summary>
		/// Verarbeitet die Eingabe und reicht das Ergebnis weiter an das nächste Element.
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <seealso cref="IFilter{TIn,TOut}.Filter"/>
		public virtual void Process(TIn input)
		{
			SetProcessingState(ProcessState.Filtering);
			TOut result = Filter(input);
			SetProcessingState(ProcessState.Dispatching);
			Follower.Process(result);
			SetProcessingState(ProcessState.Idle);
		}
	}
}

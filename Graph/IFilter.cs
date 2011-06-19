namespace Graph
{
	/// <summary>
	/// Interface für Filter
	/// </summary>
	/// <typeparam name="TIn">Eingabedatentyp</typeparam>
	/// <typeparam name="TOut">Ausgabedatentyp</typeparam>
	public interface IFilter<in TIn, out TOut> : IAppendableDataProcessor<TIn, TOut>
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		TOut Filter(TIn input);
	}
}

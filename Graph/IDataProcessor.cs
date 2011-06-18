namespace Graph
{
	/// <summary>
	/// Interface für eine verarbeitbares Element
	/// </summary>
	/// <typeparam name="TIn">Eingabedatentyp</typeparam>
	public interface IDataProcessor<in TIn> : IProcessIndicator
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		void Process(TIn input);
	}
}

namespace Graph
{
	/// <summary>
	/// Interface für eine Senke
	/// </summary>
	/// <typeparam name="TOut">Ausgabedatentyp</typeparam>
	public interface ISource<out TOut> : IAppendable<TOut>
	{
		/// <summary>
		/// Erzeugt die Eingabe
		/// </summary>
		/// <rereturns>Die Ausgabedaten</rereturns>
		TOut Create();

		/// <summary>
		/// Erzeugt die Ausgabe und leitet sie an das nächste Element weiter
		/// </summary>
		void Process();
	}
}

namespace Graph
{
	/// <summary>
	/// Interface für eine verarbeitbares Element
	/// </summary>
	/// <typeparam name="TIn">Eingabedatentyp</typeparam>
	/// <typeparam name="TOut">Ausgabedatentyp</typeparam>
	public interface IAppendableDataProcessor<in TIn, out TOut> : IAppendable<TOut>, IDataProcessor<TIn>
	{
	}
}

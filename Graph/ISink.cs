namespace Graph
{
	/// <summary>
	/// Interface für eine Senke
	/// </summary>
	/// <typeparam name="TIn">Eingabedatentyp</typeparam>
	public interface ISink<in TIn> : IDataProcessor<TIn>
	{
	}
}

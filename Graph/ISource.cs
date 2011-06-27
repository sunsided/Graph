namespace Graph
{
	/// <summary>
	/// Interface für eine Quelle
	/// </summary>
	/// <typeparam name="TOutput">Der Ausgangsdatentyp</typeparam>
	public interface ISource<out TOutput> : IDataOutput<TOutput>
	{
	}
}

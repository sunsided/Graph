namespace Graph
{
	/// <summary>
	/// Interface für ein Passthrough-Filter
	/// </summary>
	/// <typeparam name="TData">Der Ein- und Ausgangsdatentyp</typeparam>
	public interface IPassthrough<TData> : IFilter<TData, TData>
	{
	}
}

namespace Graph
{
	/// <summary>
	/// Interface for a filter that directly passes the input to the output
	/// </summary>
	/// <typeparam name="TData">The input and output data type</typeparam>
	public interface IPassthrough<TData> : IFilter<TData, TData>
	{
	}
}

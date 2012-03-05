namespace Graph
{
	/// <summary>
	/// Interface for a data sink
	/// </summary>
	/// <typeparam name="TInput">The input data type</typeparam>
	public interface ISink<in TInput> : IDataInput<TInput>
	{
	}
}

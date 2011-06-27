namespace Graph
{
	/// <summary>
	/// Interface für ein Filter
	/// </summary>
	/// <typeparam name="TInput">Der Eingangsdatentyp</typeparam>
	/// <typeparam name="TOutput">Der Ausgangsdatentyp</typeparam>
	public interface IFilter<in TInput, out TOutput> : IDataInput<TInput>, IDataOutput<TOutput>
	{
	}
}

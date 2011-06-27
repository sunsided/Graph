namespace Graph
{
	/// <summary>
	/// Interface für eine Senke
	/// </summary>
	/// <typeparam name="TInput">Der Eingangsdatentyp</typeparam>
	public interface ISink<in TInput> : IDataInput<TInput>
	{
	}
}

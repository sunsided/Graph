namespace Graph
{
	/// <summary>
	/// Interface für ein Filter mit zwei Eingängen unterschiedlichen Datentyps
	/// </summary>
	/// <typeparam name="TInput1">Der erste Eingangsdatentyp</typeparam>
	/// <typeparam name="TInput2">Der zweite Eingangsdatentyp</typeparam>
	/// <typeparam name="TOutput">Der Ausgangsdatentyp</typeparam>
	public interface IDualInFilter<in TInput1, in TInput2, out TOutput> : ISource<TOutput>
	{
		/// <summary>
		/// Der erste Input
		/// </summary>
		ISink<TInput1> Input1 { get; }

		/// <summary>
		/// Der zweite Input
		/// </summary>
		ISink<TInput2> Input2 { get; }
	}

	/// <summary>
	/// Interface für ein Filter mit zwei Eingängen gleichen Datentyps
	/// </summary>
	/// <typeparam name="TInput">Der Eingangsdatentyp</typeparam>
	/// <typeparam name="TOutput">Der Ausgangsdatentyp</typeparam>
	public interface IDualInFilter<in TInput, out TOutput> : IDualInFilter<TInput, TInput, TOutput>
	{
	}
}

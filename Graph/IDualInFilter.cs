namespace Graph
{
    /// <summary>
    /// Interface for a filter with two different inputs.
    /// </summary>
    /// <typeparam name="TInput1">The first input data type</typeparam>
    /// <typeparam name="TInput2">The second input data type</typeparam>
    /// <typeparam name="TOutput">The output data type</typeparam>
    /// <seealso cref="IDualInFilter{TInput,TOutput}"/>
    public interface IDualInFilter<in TInput1, in TInput2, out TOutput> : ISource<TOutput>
    {
        /// <summary>
        /// Gets the fist input
        /// </summary>
        ISink<TInput1> Input1 { get; }

        /// <summary>
        /// Gets the second input.
        /// </summary>
        ISink<TInput2> Input2 { get; }
    }

    /// <summary>
    ///  Interface for a filter with two identical inputs.
    /// </summary>
    /// <typeparam name="TInput">The input data type</typeparam>
    /// <typeparam name="TOutput">The output dara type</typeparam>
    /// <seealso cref="IDualInFilter{TInput1,TInput2,TOutput}"/>
    public interface IDualInFilter<in TInput, out TOutput> : IDualInFilter<TInput, TInput, TOutput>
    {
    }
}

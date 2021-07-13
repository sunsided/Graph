namespace Graph
{
    /// <summary>
    /// Interface for a filter
    /// </summary>
    /// <typeparam name="TInput">The input data type</typeparam>
    /// <typeparam name="TOutput">The output data type</typeparam>
    public interface IFilter<in TInput, out TOutput> : IDataInput<TInput>, IDataOutput<TOutput>
    {
    }
}

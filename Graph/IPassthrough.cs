namespace Graph
{
    /// <summary>
    /// Interface for filters that directly pass the input to the output.
    /// </summary>
    /// <typeparam name="TData">The input and output data type</typeparam>
    public interface IPassthrough<TData> : IFilter<TData, TData>
    {
    }
}

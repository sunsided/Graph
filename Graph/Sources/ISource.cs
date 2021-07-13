namespace Graph.Sources
{
    /// <summary>
    /// Interface for data sources.
    /// </summary>
    /// <typeparam name="TOutput">The output data type</typeparam>
    public interface ISource<out TOutput> : IDataOutput<TOutput>
    {
    }
}

namespace Graph
{
    /// <summary>
    /// Interface for a data source
    /// </summary>
    /// <typeparam name="TOutput">The output data type</typeparam>
    public interface ISource<out TOutput> : IDataOutput<TOutput>
    {
    }
}

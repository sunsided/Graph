namespace Graph
{
    /// <summary>
    /// Interface for data sinks.
    /// </summary>
    /// <typeparam name="TInput">The input data type</typeparam>
    public interface ISink<in TInput> : IDataInput<TInput>
    {
    }
}

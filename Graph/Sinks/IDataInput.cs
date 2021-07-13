using System.Diagnostics.Contracts;

namespace Graph.Sinks
{
    /// <summary>
    /// Interface for elements that take data.
    /// </summary>
    /// <typeparam name="TData">The input data type</typeparam>
    public interface IDataInput<in TData>
    {
        /// <summary>
        /// Gets the length of the input queue.
        /// </summary>
        int InputQueueLength { [Pure] get; }

        /// <summary>
        /// Registers an input value.
        /// <para>
        /// If the input queue has free slots, this call is non-blocking, otherwise it is blocked
        /// until a queue slot is freed.
        /// </para>
        /// </summary>
        /// <param name="input">The value to register.</param>
        bool RegisterInput(TData input);
    }
}

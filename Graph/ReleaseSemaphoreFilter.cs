using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
    /// <summary>
    /// A filter that releases a <see cref="Semaphore"/> whenever it passes elements on to the output.
    /// </summary>
    /// <typeparam name="T">The input and output type.</typeparam>
    /// <seealso cref="WaitEventFilter{T}"/>
    public sealed class ReleaseSemaphoreFilter<T> : PassthroughFilter<T>
    {
        /// <summary>
        /// Das WaitHandle
        /// </summary>
        public WaitHandle WaitHandle { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetEventFilter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        public ReleaseSemaphoreFilter(Semaphore handle)
        {
            Contract.Requires(handle != null);
            WaitHandle = handle;
        }

        /// <summary>
        /// Processes the input.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <returns>The <paramref name="input"/> value.</returns>
        /// <remarks>This method implements only the filter functionality..</remarks>
        public override T Filter(T input)
        {
            ((Semaphore)WaitHandle).Release();
            return input;
        }
    }
}

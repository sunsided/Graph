using System.Diagnostics.Contracts;
using Graph.Sinks;

namespace Graph.Sources
{
    /// <summary>
    /// Interface for elements that produce data.
    /// </summary>
    /// <typeparam name="TOutput">The output data type.</typeparam>
    public interface IDataOutput<out TOutput>
    {
        /// <summary>
        /// Gets the number of output processors.
        /// </summary>
        int OutputProcessorCount
        {
            [Pure]
            get;
        }

        /// <summary>
        /// Registers a processor for the output values.
        /// </summary>
        /// <param name="outputProcessor">The processor to register.</param>
        /// <returns><see langword="true" /> if the processor could be attached successfully; <see langword="false" /> otherwise</returns>
        bool AttachOutput(IDataInput<TOutput> outputProcessor);

        /// <summary>
        /// Unregisters a processor.
        /// </summary>
        /// <param name="outputProcessor">The processor to be removed.</param>
        /// <returns><see langword="true" /> if the processor could be detached successfully; <see langword="false" /> otherwise</returns>
        bool DetachOutput(IDataInput<TOutput> outputProcessor);
    }
}

using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Interface for elements that produce data.
	/// </summary>
	/// <typeparam name="TOutput">The output data type.</typeparam>
	public interface IDataOutput<out TOutput>
	{
		/// <summary>
		/// The count of output processors.
		/// </summary>
		int OutputProcessorCount { [Pure] get; }

		/// <summary>
		/// Registers a processor for the output values.
		/// </summary>
		/// <param name="outputProcessor">The processor to register.</param>
		/// <returns><c>true</c> if the processor could be attached successfully; <c>false</c> otherwise</returns>
		bool AttachOutput(IDataInput<TOutput> outputProcessor);

		/// <summary>
		/// Unregisters a processor.
		/// </summary>
		/// <param name="outputProcessor">The processor to be removed.</param>
        /// <returns><c>true</c> if the processor could be detached successfully; <c>false</c> otherwise</returns>
		bool DetachOutput(IDataInput<TOutput> outputProcessor);
	}
}
using System.Diagnostics.Contracts;

namespace Graph.Logic
{
	/// <summary>
	/// Produces a logical NOR of two inputs
	/// </summary>
	public sealed class NorGate : DualInFilter<bool, bool>
	{
        /// <summary>
        /// Processes the data
        /// </summary>
        /// <param name="input1">The first input value</param>
        /// <param name="input2">The second input value</param>
        /// <param name="output">NOT (<paramref name="input1"/> OR <paramref name="input2"/>)</param>
        /// <returns>Always <c>true</c>. </returns>
		[Pure]
		protected override bool ProcessData(bool input1, bool input2, out bool output)
		{
			Contract.Ensures(Contract.Result<bool>());

			output = !(input1 || input2);
			return true;
		}
	}
}

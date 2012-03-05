using System.Diagnostics.Contracts;

namespace Graph.Logic
{
	/// <summary>
	/// Produces a logical NOT (inversion) of the input
	/// </summary>
	public sealed class NotGate : DataFilter<bool, bool>
	{
        /// <summary>
        /// Processes the data
        /// </summary>
        /// <param name="input">The first input value</param>
        /// <param name="output">NOT <paramref name="input"/></param>
        /// <returns>Always <c>true</c>. </returns>
		[Pure]
		protected override bool ProcessData(bool input, out bool output)
		{
			Contract.Ensures(Contract.Result<bool>());

			output = !input;
			return true;
		}
	}
}

using System.Diagnostics.Contracts;

namespace Graph.Filters.LogicGates
{
    /// <summary>
    /// Produces a logical <c>NOT</c> (inversion) of the input
    /// </summary>
    public sealed class NotGate : DataFilter<bool, bool>
    {
        /// <summary>
        /// Processes the data
        /// </summary>
        /// <param name="input">The first input value</param>
        /// <param name="output">NOT <paramref name="input"/></param>
        /// <returns>Always <see langword="true" />. </returns>
        [Pure]
        protected override bool ProcessData(bool input, out bool output)
        {
            Contract.Ensures(Contract.Result<bool>());

            output = !input;
            return true;
        }
    }
}

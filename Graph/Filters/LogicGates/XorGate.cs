﻿using System.Diagnostics.Contracts;

namespace Graph.Filters.LogicGates
{
    /// <summary>
    /// Produces a logical <c>XOR</c> of the inputs
    /// </summary>
    public sealed class XorGate : DualInFilter<bool, bool>
    {
        /// <summary>
        /// Processes the data
        /// </summary>
        /// <param name="input1">The first input value</param>
        /// <param name="input2">The second input value</param>
        /// <param name="output"><paramref name="input1"/> XOR <paramref name="input2"/></param>
        /// <returns>Always <see langword="true" />. </returns>
        [Pure]
        protected override bool ProcessData(bool input1, bool input2, out bool output)
        {
            Contract.Ensures(Contract.Result<bool>());

            output = input1 ^ input2;
            return true;
        }
    }
}

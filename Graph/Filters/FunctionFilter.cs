using System;
using System.Diagnostics.Contracts;

namespace Graph.Filters
{
    /// <summary>
    /// Filter that applies a mapping function to each input.
    /// </summary>
    public sealed class FunctionFilter<TInput, TOutput> : DataFilter<TInput, TOutput>
    {
        /// <summary>
        /// The function to call.
        /// </summary>
        private readonly Func<FunctionFilter<TInput, TOutput>, TInput, TOutput> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionFilter{TInput,TOutput}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public FunctionFilter(Func<TInput, TOutput> func)
            : this((_, input) => func(input))
        {
            Contract.Requires(func != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionFilter{TInput,TOutput}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public FunctionFilter(Func<FunctionFilter<TInput, TOutput>, TInput, TOutput> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        /// <summary>
        /// Maps the function to the input
        /// </summary>
        /// <param name="input">The input value</param>
        /// <param name="output">The output value</param>
        /// <returns>Always <see langword="true" /> </returns>
        protected override bool ProcessData(TInput input, out TOutput output)
        {
            output = _func(this, input);
            return true;
        }
    }
}

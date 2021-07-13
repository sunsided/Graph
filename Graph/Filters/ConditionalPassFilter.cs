using System;
using System.Diagnostics.Contracts;

namespace Graph.Filters
{
    /// <summary>
    /// Filter that passes on data only if a test function evaluates to <see langword="true" />.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public sealed class ConditionalPassFilter<TData> : DataFilter<TData, TData>, IPassthrough<TData>
    {
        /// <summary>
        /// The test function.
        /// </summary>
        private readonly Func<ConditionalPassFilter<TData>, TData, bool> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionFilter{TInput,TOutput}"/> class.
        /// </summary>
        /// <param name="evaluationFunc">The evaluation function.</param>
        public ConditionalPassFilter(Func<TData, bool> evaluationFunc)
            : this((ignored, input) => evaluationFunc(input) )
        {
            Contract.Requires(evaluationFunc != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionFilter{TInput,TOutput}"/> class.
        /// </summary>
        /// <param name="evaluationFunc">The evaluation function.</param>
        public ConditionalPassFilter(Func<ConditionalPassFilter<TData>, TData, bool> evaluationFunc)
        {
            Contract.Requires(evaluationFunc != null);
            _func = evaluationFunc;
        }

        /// <summary>
        /// Evaluates the test function.
        /// </summary>
        /// <param name="input">The input value</param>
        /// <param name="output">Always <paramref name="input"/>.</param>
        /// <returns><see langword="true" /> or <see langword="false" /> depending on the function result</returns>
        protected override bool ProcessData(TData input, out TData output)
        {
            output = input;
            return _func(this, input);
        }
    }
}

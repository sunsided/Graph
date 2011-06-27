using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das einen Wert nur weiterreicht, wenn eine Entscheidungsfunktion mit
	/// <c>true</c> evaluiert.
	/// </summary>
	public sealed class ConditionalPassFilter<TData> : DataFilter<TData, TData>, IPassthrough<TData>
	{
		/// <summary>
		/// Die auszuführende Aktion
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
		/// Wertet die Entscheidungsfunktion aus und reicht den Wert weiter
		/// </summary>
		/// <param name="input">Der Eingabewert</param>
		/// <param name="output">Immer <paramref name="input"/>.</param>
		/// <returns><c>true</c> oder <c>false</c>, abhängig von der Entscheidungsfunktion</returns>
		protected override bool ProcessData(TData input, out TData output)
		{
			output = input;
			return _func(this, input);
		}
	}
}

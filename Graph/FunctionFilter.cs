using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Basisklasse für ein Datenverarbeitungselement mit Ausgang
	/// </summary>
	public sealed class FunctionFilter<TInput, TOutput> : DataFilter<TInput, TOutput>
	{
		/// <summary>
		/// Die auszuführende Aktion
		/// </summary>
		private readonly Func<FunctionFilter<TInput, TOutput>, TInput, TOutput> _func;

		/// <summary>
		/// Initializes a new instance of the <see cref="FunctionFilter{TInput,TOutput}"/> class.
		/// </summary>
		/// <param name="func">The function.</param>
		public FunctionFilter(Func<TInput, TOutput> func)
			: this((ignored, input) => func(input) )
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
		/// Verteilt den Eingang aug die Ausgänge
		/// </summary>
		/// <param name="input">Der Eingabewert</param>
		/// <param name="output">Der Ausgabewert</param>
		/// <returns>Immer <c>true</c>.
		/// </returns>
		protected override bool ProcessData(TInput input, out TOutput output)
		{
			output = _func(this, input);
			return true;
		}
	}
}

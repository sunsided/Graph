using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente mittels einer Delegate bearbeitet
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	/// <typeparam name="TOut">Ausgabeparameter</typeparam>
	public class DelegateFilter<TIn, TOut> : FilterBase<TIn, TOut>
	{
		/// <summary>
		/// Die zu verwendende Filterfunktion
		/// </summary>
		private readonly Func<TIn, TOut> _filter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public DelegateFilter(Func<TIn, TOut> filter)
		{
			Contract.Requires(filter != null);
			_filter = filter;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		[Pure]
		public override TOut Filter(TIn input)
		{
			return _filter(input);
		}
	}

	/// <summary>
	/// Filter, das Elemente mittels einer Delegate bearbeitet
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public class DelegateFilter<T> : FilterBase<T, T>
	{
		/// <summary>
		/// Die zu verwendende Filterfunktion
		/// </summary>
		private readonly Func<T, T> _filter;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public DelegateFilter(Func<T, T> filter)
		{
			Contract.Requires(filter != null);
			_filter = filter;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		[Pure]
		public override T Filter(T input)
		{
			return _filter(input);
		}
	}
}

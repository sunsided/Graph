using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente mittels einer Delegate bearbeitet
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	/// <typeparam name="TOut">Ausgabeparameter</typeparam>
	public sealed class DelegateFilter<TIn, TOut> : FilterBase<TIn, TOut>
	{
		/// <summary>
		/// Die zu verwendende Filterfunktion
		/// </summary>
		private readonly Func<DelegateFilter<TIn, TOut>, TIn, TOut> _filter;

		/// <summary>
		/// Benutzerdefiniertes Objekt
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public DelegateFilter(Func<TIn, TOut> filter)
			: this(filter, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="tag">Benutzerdefiniertes Tag</param>
		public DelegateFilter(Func<TIn, TOut> filter, object tag)
			: this((sender, @in) => filter(@in), tag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public DelegateFilter(Func<DelegateFilter<TIn, TOut>, TIn, TOut> filter)
			: this(filter, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="tag">Benutzerdefiniertes Tag</param>
		public DelegateFilter(Func<DelegateFilter<TIn, TOut>, TIn, TOut> filter, object tag)
		{
			Contract.Requires(filter != null);
			_filter = filter;
			Tag = tag;
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
			return _filter(this, input);
		}
	}

	/// <summary>
	/// Filter, das Elemente mittels einer Delegate bearbeitet
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class DelegateFilter<T> : DelegateFilter<T, T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public DelegateFilter(Func<T, T> filter) : base(filter)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="tag">The tag.</param>
		public DelegateFilter(Func<T, T> filter, object tag) : base(filter, tag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		public DelegateFilter(Func<DelegateFilter<T, T>, T, T> filter) : base(filter)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="tag">The tag.</param>
		public DelegateFilter(Func<DelegateFilter<T, T>, T, T> filter, object tag) : base(filter, tag)
		{
		}
	}
}

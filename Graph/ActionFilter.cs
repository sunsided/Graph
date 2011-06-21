using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das eine Aktion ausführt und Elemente weiterreicht
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	/// <seealso cref="DelegateFilter{T}"/>
	/// <seealso cref="ActionSink{T}"/>
	/// <seealso cref="ConditionalPassthroughFilter{T}"/>
	public sealed class ActionFilter<T> : PassthroughFilter<T>
	{
		/// <summary>
		/// Die zu verwendende Aktion
		/// </summary>
		private readonly Action<ActionFilter<T>, T> _action;

		/// <summary>
		/// Benutzerdefiniertes Objekt
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public ActionFilter(Action<T> action)
			: this(action, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="tag">Benutzerdefiniertes Tag</param>
		public ActionFilter(Action<T> action, object tag)
			: this((sender, @in) => action(@in), tag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public ActionFilter(Action<ActionFilter<T>, T> action)
			: this(action, null)
		{
			Contract.Requires(action != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="tag">Benutzerdefiniertes Tag</param>
		public ActionFilter(Action<ActionFilter<T>, T> action, object tag)
		{
			Contract.Requires(action != null);
			_action = action;
			Tag = tag;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		public override T Filter(T input)
		{
			_action(this, input);
			return input;
		}
	}
}

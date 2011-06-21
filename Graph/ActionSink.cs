using System;

namespace Graph
{
	/// <summary>
	/// Senke, die eine Aktion ausführt
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	/// <seealso cref="DelegateFilter{T}"/>
	/// <seealso cref="ActionFilter{T}"/>
	public sealed class ActionSink<TIn> : StateBase, ISink<TIn>
	{
		/// <summary>
		/// Die auszuführende Aktion
		/// </summary>
		private readonly Action<ActionSink<TIn>, TIn> _action;

		/// <summary>
		/// Benutzerdefiniertes Objekt
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionSink{TIn}"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public ActionSink(Action<TIn> action)
			: this(action, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionSink{TIn}"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="tag">Benutzerdefiniertes Tag</param>
		public ActionSink(Action<TIn> action, object tag)
			: this((sender, value) => action(value), tag)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionSink{TIn}"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public ActionSink(Action<ActionSink<TIn>, TIn> action)
			: this(action, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionSink{TIn}"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <param name="tag">Benutzerdefiniertes Tag</param>
		public ActionSink(Action<ActionSink<TIn>, TIn> action, object tag)
		{
			_action = action;
			Tag = tag;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		public void Process(TIn input)
		{
			SetProcessingState(ProcessState.Dispatching, input);
			_action(this, input);
			SetProcessingState(ProcessState.Idle, null);
		}
	}
}

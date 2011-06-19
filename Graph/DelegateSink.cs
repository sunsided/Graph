using System;

namespace Graph
{
	/// <summary>
	/// Senke, die Daten verwirft
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter</typeparam>
	public class DelegateSink<TIn> : StateBase, ISink<TIn>
	{
		/// <summary>
		/// Die auszuführende Aktion
		/// </summary>
		private readonly Action<DelegateSink<TIn>, TIn> _action;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateSink&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public DelegateSink(Action<TIn> action)
			: this((sender, value) => action(value))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateSink&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="action">The action.</param>
		public DelegateSink(Action<DelegateSink<TIn>, TIn> action)
		{
			_action = action;
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

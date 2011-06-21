using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;

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

	/// <summary>
	/// Helfermethoden für <see cref="ActionFilter{T}"/>
	/// </summary>
	public static class ActionFilterHelper
	{
		/// <summary>
		/// Gibt eine Trace-Meldung aus
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="message">Die auszugebende Meldung</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ActionFilter<T> AttachTraceOutput<T>(this IFilter<T, T> processor, string message)
		{
			Contract.Requires(processor != null);
			Contract.Requires(message != null);
			Contract.Ensures(Contract.Result<ActionFilter<T>>() != null);

			return new ActionFilter<T>(value =>
			                           	{
			                           		Trace.WriteLine(message);
			                           		processor.Process(value);
			                           	});
		}

		/// <summary>
		/// Gibt eine Debug-Meldung aus
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="message">Die auszugebende Meldung</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ActionFilter<T> AttachDebugOutput<T>(this IFilter<T, T> processor, string message)
		{
			Contract.Requires(processor != null);
			Contract.Requires(message != null);
			Contract.Ensures(Contract.Result<ActionFilter<T>>() != null);

			return new ActionFilter<T>(value =>
			                           	{
			                           		Debug.WriteLine(message);
			                           		processor.Process(value);
			                           	});
		}

		/// <summary>
		/// Gibt eine Debug-Meldung aus
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="writer">Der zu verwendende Writer</param>
		/// <param name="message">Die auszugebende Meldung</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ActionFilter<T> AttachOutput<T>(this IFilter<T, T> processor, TextWriter writer, string message)
		{
			Contract.Requires(processor != null);
			Contract.Requires(message != null);
			Contract.Requires(writer != null);
			Contract.Ensures(Contract.Result<ActionFilter<T>>() != null);

			return new ActionFilter<T>(value =>
			{
				writer.WriteLine(message);
				processor.Process(value);
			});
		}
	}
}

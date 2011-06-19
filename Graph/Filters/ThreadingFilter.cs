using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph.Filters
{
	/// <summary>
	/// Filter, welches die Weiterverarbeitung in einem neuen Thread durchführt.
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter, identisch mit <typeparamref name="TOut"/></typeparam>
	/// <typeparam name="TOut">Ausgabeparameter, identisch mit <typeparamref name="TIn"/>></typeparam>
	public sealed class ThreadingFilter<TIn, TOut> : PassthroughFilter<TIn, TOut> where TIn : TOut
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		public ThreadingFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		public ThreadingFilter(IDataProcessor<TOut> next)
		{
			Contract.Assume(next != (IDataProcessor<TIn>)this);
			Follower = next;
		}

		/// <summary>
		/// Führt die Weit
		/// </summary>
		/// <param name="input"></param>
		public override void Process(TIn input)
		{
			// Follower ermitteln
			IDataProcessor<TOut> follower = Follower;

			// Kopieren
			SetProcessingState(ProcessState.Filtering, input);
			TOut result = Filter(input);

			// Delegat erzeugen und aufrufen.
			SetProcessingState(ProcessState.Dispatching, input);
			WaitCallback callback = delegate { follower.Process(result); };
			ThreadPool.QueueUserWorkItem(callback);

			// Fertig
			SetProcessingState(ProcessState.Idle, default(TIn));
		}
	}
}

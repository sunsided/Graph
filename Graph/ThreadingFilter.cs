using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
	/// <summary>
	/// Filter, welches die Weiterverarbeitung in einem neuen Thread durchführt.
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class ThreadingFilter<T> : PassthroughFilter<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TInt&gt;"/> class.
		/// </summary>
		public ThreadingFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		public ThreadingFilter(IDataProcessor<T> next)
		{
			Contract.Assume(next != this);
			Follower = next;
		}

		/// <summary>
		/// Führt die Weit
		/// </summary>
		/// <param name="input"></param>
		public override void Process(T input)
		{
			// Follower ermitteln
			IDataProcessor<T> follower = Follower;

			// Kopieren
			SetProcessingState(ProcessState.Filtering, input);
			T result = Filter(input);

			// Delegat erzeugen und aufrufen.
			SetProcessingState(ProcessState.Dispatching, input);
			WaitCallback callback = delegate { follower.Process(result); };
			ThreadPool.QueueUserWorkItem(callback);

			// Fertig
			SetProcessingState(ProcessState.Idle, null);
		}
	}
}

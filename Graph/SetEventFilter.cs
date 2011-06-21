using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente weiterreicht und dabei ein <see cref="EventWaitHandle"/> setzt
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	/// <seealso cref="ResetEventFilter{T}"/>
	/// <seealso cref="WaitEventFilter{T}"/>
	/// <seealso cref="ReleaseSemaphoreFilter{T}"/>
	public sealed class SetEventFilter<T> : PassthroughFilter<T>
	{
		/// <summary>
		/// Das WaitHandle
		/// </summary>
		public WaitHandle WaitHandle { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SetEventFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="handle">The handle.</param>
		public SetEventFilter(EventWaitHandle handle)
		{
			Contract.Requires(handle != null);
			WaitHandle = handle;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		public override T Filter(T input)
		{
			((EventWaitHandle) WaitHandle).Set();
			return input;
		}
	}
}

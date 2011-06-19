using System.Threading;

namespace Graph
{
	/// <summary>
	/// Filter, das auf ein <see cref="WaitHandle"/> wartet und dann die Elemente weiterreicht
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class WaitEventFilter<T> : PassthroughFilter<T>
	{
		/// <summary>
		/// Das WaitHandle
		/// </summary>
		public WaitHandle WaitHandle { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SetEventFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="handle">The handle.</param>
		public WaitEventFilter(WaitHandle handle)
		{
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
			WaitHandle.WaitOne();
			return input;
		}
	}
}

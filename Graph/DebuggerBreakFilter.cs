using System.Diagnostics;

namespace Graph
{
	/// <summary>
	/// Filter, das einen Debugger-Break absetzt
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class DebuggerBreakFilter<T> : PassthroughFilter<T>
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		public override T Filter(T input)
		{
			if(Debugger.IsAttached) Debugger.Break();
			return input;
		}
	}
}

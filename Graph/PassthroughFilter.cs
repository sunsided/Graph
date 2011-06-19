using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente nur weiterreicht
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public class PassthroughFilter<T> : FilterBase<T, T>
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		[Pure]
		public override T Filter(T input)
		{
			return input;
		}
	}
}

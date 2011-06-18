using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente nur weiterreicht
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter, identisch mit <typeparamref name="TOut"/></typeparam>
	/// <typeparam name="TOut">Ausgabeparameter, identisch mit <typeparamref name="TIn"/>></typeparam>
	public class PassthroughFilter<TIn, TOut> : FilterBase<TIn, TOut> where TIn: TOut
	{
		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		[Pure]
		public override TOut Filter(TIn input)
		{
			return input;
		}
	}
}

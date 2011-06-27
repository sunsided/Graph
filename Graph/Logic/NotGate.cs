using System;
using System.Diagnostics.Contracts;

namespace Graph.Logic
{
	/// <summary>
	/// Ein NOT-Gatter
	/// </summary>
	public sealed class NotGate : DataFilter<bool, bool>
	{
		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="input">Der Eingabewert</param>
		/// <param name="output">NOT <paramref name="input"/></param>
		/// <returns>Immer <c>true</c>. </returns>
		[Pure]
		protected override bool ProcessData(bool input, out bool output)
		{
			Contract.Ensures(Contract.Result<bool>());

			output = !input;
			return true;
		}
	}
}

using System.Diagnostics.Contracts;

namespace Graph.Logic
{
	/// <summary>
	/// Ein XNOR-Gatter
	/// </summary>
	public sealed class XnorGate : DualInFilter<bool, bool>
	{
		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="input1">Der erste Eingabewert</param>
		/// <param name="input2">Der zweite Eingabewert</param>
		/// <param name="output">NOT (<paramref name="input1"/> XOR <paramref name="input2"/>)</param>
		/// <returns>Immer <c>true</c>. </returns>
		[Pure]
		protected override bool ProcessData(bool input1, bool input2, out bool output)
		{
			Contract.Ensures(Contract.Result<bool>());

			output = !(input1 ^ input2);
			return true;
		}
	}
}

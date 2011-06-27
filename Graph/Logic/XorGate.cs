using System.Diagnostics.Contracts;

namespace Graph.Logic
{
	/// <summary>
	/// Ein XOR-Gatter
	/// </summary>
	public sealed class XorGate : DualInFilter<bool, bool>
	{
		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="input1">Der erste Eingabewert</param>
		/// <param name="input2">Der zweite Eingabewert</param>
		/// <param name="output"><paramref name="input1"/> XOR <paramref name="input2"/></param>
		/// <returns>Immer <c>true</c>. </returns>
		[Pure]
		protected override bool ProcessData(bool input1, bool input2, out bool output)
		{
			Contract.Ensures(Contract.Result<bool>());

			output = input1 ^ input2;
			return true;
		}
	}
}

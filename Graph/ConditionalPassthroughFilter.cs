using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente nur weiterreicht
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public class ConditionalPassthroughFilter<T> : PassthroughFilter<T>
	{
		/// <summary>
		/// Die Entscheidungsfunktion
		/// </summary>
		private readonly Func<T, bool> _decisionFunc;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalPassthroughFilter&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="decisionFunc">Die Entscheidungsfunktion.</param>
		public ConditionalPassthroughFilter(Func<T, bool> decisionFunc)
		{
			_decisionFunc = decisionFunc;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		[Pure]
		public sealed override T Filter(T input)
		{
			return input;
		}

		/// <summary>
		/// Entscheidet über den gegebenen Eingabewert
		/// </summary>
		/// <param name="input">Der Eingabewert.</param>
		/// <returns><c>true</c>, wenn der Wert weitergereicht werden soll, ansonsten <c>false</c></returns>
		protected virtual bool Decide(T input)
		{
			return true;
		}

		/// <summary>
		/// Entscheidet, ob die Daten weitergereicht werden dürfen
		/// </summary>
		/// <param name="input">The input.</param>
		public sealed override void Process(T input)
		{
			SetProcessingState(ProcessState.Filtering, input);

			// Entscheidungsfunktion auswerten
			bool passthrough = true;
			if (_decisionFunc != null) passthrough &= _decisionFunc(input);
			passthrough &= Decide(input);

			// Wenn erlaubt, durchreichen
			if (passthrough) base.Process(input);
		}
	}
}

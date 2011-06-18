using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, das Elemente nur weiterreicht
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter, identisch mit <typeparamref name="TOut"/></typeparam>
	/// <typeparam name="TOut">Ausgabeparameter, identisch mit <typeparamref name="TIn"/>></typeparam>
	public abstract class FilterBase<TIn, TOut> : IFilter<TIn, TOut> where TIn: TOut
	{
		/// <summary>
		/// Das nächste Element
		/// </summary>
		private IDataProcessor<TOut> _follower;

		/// <summary>
		/// Liefert das Nachfolgeelement
		/// </summary>
		public IDataProcessor<TOut> Follower
		{
			[Pure] get
			{
				Contract.Ensures(Contract.Result<IDataProcessor<TOut>>() != null);
				return _follower;
			}
			set
			{
				Contract.Requires(value != null);
				_follower = value;
			}
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <remarks>Hier wird auschließlich die Filterlogik implementiert.</remarks>
		public abstract TOut Filter(TIn input);

		/// <summary>
		/// Verarbeitet die Eingabe und reicht das Ergebnis weiter an das nächste Element.
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		/// <returns>Das Ergebnis</returns>
		/// <seealso cref="IFilter{TIn,TOut}.Filter"/>
		public void Process(TIn input)
		{
			TOut result = Filter(input);
			_follower.Process(result);
		}

		/// <summary>
		/// Hängt ein Element an
		/// </summary>
		/// <param name="element">Das anzuhängende Element</param>
		public void Append(IDataProcessor<TOut> element)
		{
			lock (_follower) _follower = element;
		}

		/// <summary>
		/// Invariantenprüfung
		/// </summary>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(_follower != null);
		}
	}
}

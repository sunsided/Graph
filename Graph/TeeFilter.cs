using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, der den Eingang auf mehrere Ausgänge gibt.
	/// </summary>
	/// <typeparam name="TIn">Eingabeparameter, identisch mit <typeparamref name="TOut"/></typeparam>
	/// <typeparam name="TOut">Ausgabeparameter, identisch mit <typeparamref name="TIn"/>></typeparam>
	/// <remarks>
	/// Die Methode ist threadsicher, da alle Ausgänge sequentiell abgearbeitet werden. 
	/// Das Nachschalten eines Asynchronfilters kann die Threadsicherheit auflösen!
	/// </remarks>
	public class TeeFilter<TIn, TOut> : ISink<TIn>, IAppendable<TOut> where TIn : TOut
	{
		/// <summary>
		/// Die Liste der angehängten Elemente
		/// </summary>
		private readonly List<IDataProcessor<TOut>> _elementList = new List<IDataProcessor<TOut>>();

		/// <summary>
		/// Liefert die Anzahl der registrierten Ausgänge
		/// </summary>
		[Pure]
		public int Count
		{
			get { lock(_elementList) return _elementList.Count; }
		}

		/// <summary>
		/// Löscht alle Ausgänge
		/// </summary>
		public void Clear()
		{
			lock (_elementList) _elementList.Clear();
		}

		/// <summary>
		/// Hängt ein Element an
		/// </summary>
		/// <param name="element">Der anzuhängende Element</param>
		void IAppendable<TOut>.Append(IDataProcessor<TOut> element)
		{
			Append(element);
		}

		/// <summary>
		/// Hängt ein Element an
		/// </summary>
		/// <param name="sink">Das anzuhängende Element</param>
		public TeeFilter<TIn, TOut> Append(IDataProcessor<TOut> sink)
		{
			Contract.Ensures(Contract.Result<TeeFilter<TIn, TOut>>() != null);
			lock (_elementList)
			{
				if (_elementList.Contains(sink)) throw new ArgumentException("Senke bereits registriert.", "sink");
				_elementList.Add(sink);
			}
			return this;
		}
		
		/// <summary>
		/// Entfernt das angegebene Element aus der Liste
		/// </summary>
		/// <param name="element">Das zu entfernende Element</param>
		/// <returns><c>true</c>, wenn das Element entfernt wurde, ansonsten <c>false</c></returns>
		public bool RemoveElement(IDataProcessor<TOut> element)
		{
			lock(_elementList) return _elementList.Remove(element);
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		public void Process(TIn input)
		{
			lock (_elementList)
			{
				// Die Liste durchlaufen
				for (int i=0; i<_elementList.Count; ++i)
				{
					IDataProcessor<TOut> element = _elementList[i];
					element.Process(input);
				}
			}
		}

		/// <summary>
		/// Invariantenprüfung
		/// </summary>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(_elementList != null);
			Contract.Invariant(Contract.ForAll(_elementList, element => element != null));
		}
	}
}

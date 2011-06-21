using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Filter, der den Eingang auf mehrere Ausgänge gibt.
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	/// <remarks>
	/// Die Methode ist threadsicher, da alle Ausgänge sequentiell abgearbeitet werden. 
	/// Das Nachschalten eines Asynchronfilters kann die Threadsicherheit auflösen!
	/// </remarks>
	public sealed class TeeFilter<T> : StateBase, IFilter<T, T>, ISink<T>
	{
		/// <summary>
		/// Die Liste der angehängten Elemente
		/// </summary>
		private readonly List<IDataProcessor<T>> _elementList = new List<IDataProcessor<T>>();
		
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
		/// <param name="sink">Das anzuhängende Element</param>
		public void Append(IDataProcessor<T> sink)
		{
			lock (_elementList)
			{
				if (_elementList.Contains(sink)) throw new ArgumentException("Senke bereits registriert.", "sink");
				_elementList.Add(sink);
			}
		}

		/// <summary>
		/// Hängt ein Element an
		/// </summary>
		/// <param name="sink">Das anzuhängende Element</param>
		public IAppendable<TOut> Append<TOut>(IAppendableDataProcessor<T, TOut> sink)
		{
			lock (_elementList)
			{
				if (_elementList.Contains(sink)) throw new ArgumentException("Senke bereits registriert.", "sink");
				_elementList.Add(sink);
			}
			return sink;
		}
		
		/// <summary>
		/// Entfernt das angegebene Element aus der Liste
		/// </summary>
		/// <param name="element">Das zu entfernende Element</param>
		/// <returns><c>true</c>, wenn das Element entfernt wurde, ansonsten <c>false</c></returns>
		public bool RemoveElement(IDataProcessor<T> element)
		{
			lock(_elementList) return _elementList.Remove(element);
		}

		/// <summary>
		/// Unbenutzt
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		T IFilter<T, T>.Filter(T input)
		{
			return input;
		}

		/// <summary>
		/// Verarbeitet die Eingabe
		/// </summary>
		/// <param name="input">Der zu verarbeitende Wert</param>
		public void Process(T input)
		{
			try
			{
				SetProcessingState(ProcessState.Dispatching, input);
				lock (_elementList)
				{
					// Die Liste durchlaufen
					for (int i = 0; i < _elementList.Count; ++i)
					{
						IDataProcessor<T> element = _elementList[i];
						element.Process(input);
					}
				}

			}
			finally
			{
				SetProcessingState(ProcessState.Idle, null);
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

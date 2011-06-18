using System;
using System.Diagnostics.Contracts;

namespace Graph
{
	public abstract class ElementBase<T>
	{
		/// <summary>
		/// Das nächste Element
		/// </summary>
		private IDataProcessor<T> _follower = new TerminatorSink<T>();

		/// <summary>
		/// Objekt für Threadsynchronisierung
		/// </summary>
		public readonly object SyncRoot = new object();

		/// <summary>
		/// Liefert das Nachfolgeelement
		/// </summary>
		public IDataProcessor<T> Follower
		{
			[Pure] get
			{
				Contract.Ensures(Contract.Result<IDataProcessor<T>>() != null);
				Contract.Ensures(Contract.Result<IDataProcessor<T>>() != this);
				return _follower;
			}
			set
			{
				Contract.Requires(value != null);
				Contract.Requires(value != this);
				_follower = value;
			}
		}

		/// <summary>
		/// Ermittelt den Zustand des Objektes
		/// </summary>
		public ProcessState State
		{
			[Pure] get; protected set; 
		}

		/// <summary>
		/// Hängt ein Element an
		/// </summary>
		/// <param name="element">Das anzuhängende Element</param>
		public void Append(IDataProcessor<T> element)
		{
			if (element == this) throw new ArgumentException("Kann nicht an sich selbst anhängen.");
			Contract.Assume(element != this);
			lock (_follower) _follower = element;
		}

		/// <summary>
		/// Invariantenprüfung
		/// </summary>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(_follower != null);
			Contract.Invariant(_follower != this);
		}

		/// <summary>
		/// Setzt den Bearbeitungsstatus
		/// </summary>
		/// <param name="state">Der Zustand, in dem sich das Element befindet</param>
		protected void SetProcessingState(ProcessState state)
		{
			Contract.Ensures(State == state);
			State = state;
		}
	}
}
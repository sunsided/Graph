using System;
using System.Diagnostics.Contracts;
using Graph.Sinks;

namespace Graph
{
	/// <summary>
	/// Basisklasse für Elemente
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ElementBase<T> : StateBase, IAppendable<T>
	{
		/// <summary>
		/// Das nächste Element
		/// </summary>
		private IDataProcessor<T> _follower = new TerminatorSink<T>();

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
		/// Hängt ein Element an
		/// </summary>
		/// <param name="element">Das anzuhängende Element</param>
		public IAppendable<TOut> Append<TOut>(IAppendableDataProcessor<T, TOut> element)
		{
			if (element == this) throw new ArgumentException("Kann nicht an sich selbst anhängen.");
			Contract.Assume(element != this);
			lock (_follower) _follower = element;
			return element;
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
	}
}
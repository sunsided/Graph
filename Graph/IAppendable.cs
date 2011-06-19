using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Interface für Elemente, an die ein Filter angehängt werden kann
	/// </summary>
	/// <typeparam name="TData">Der zu verarbeitende Datentyp</typeparam>
	[ContractClass(typeof(AppendableContract<>))]
	public interface IAppendable<out TData>
	{
		/// <summary>
		/// Hängt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TData">Eingabedatentyp des nächsten Elementes</typeparam>
		/// <param name="element">Der anzuhängende Filter</param>
		void Append(IDataProcessor<TData> element);

		/// <summary>
		/// Hängt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TData">Eingabedatentyp des nächsten Elementes</typeparam>
		/// <param name="element">Der anzuhängende Filter</param>
		IAppendable<TOutNext> Append<TOutNext>(IAppendableDataProcessor<TData, TOutNext> element);
	}

	/// <summary>
	/// Interface für Elemente, an die ein Filter angehängt werden kann
	/// </summary>
	/// <typeparam name="TData">Der zu verarbeitende Datentyp</typeparam>
	[ContractClassFor(typeof(IAppendable<>))]
	internal abstract class AppendableContract<TData> : IAppendable<TData>
	{
		/// <summary>
		/// Hängt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TData">Eingabedatentyp des nächsten Elementes</typeparam>
		/// <param name="element">Der anzuhängende Filter</param>
		public void Append(IDataProcessor<TData> element)
		{
			Contract.Requires(element != null, "Anzuhängendes Element darf nicht null sein.");
		}

		/// <summary>
		/// Hängt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TData">Eingabedatentyp des nächsten Elementes</typeparam>
		/// <param name="element">Der anzuhängende Filter</param>
		public IAppendable<TOutNext> Append<TOutNext>(IAppendableDataProcessor<TData, TOutNext> element)
		{
			Contract.Requires(element != null, "Anzuhängendes Element darf nicht null sein.");
			return null;
		}
	}
}
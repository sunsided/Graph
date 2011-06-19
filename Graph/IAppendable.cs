using System.Diagnostics.Contracts;

namespace Graph
{
	/// <summary>
	/// Interface f�r Elemente, an die ein Filter angeh�ngt werden kann
	/// </summary>
	/// <typeparam name="TData">Der zu verarbeitende Datentyp</typeparam>
	[ContractClass(typeof(AppendableContract<>))]
	public interface IAppendable<out TData>
	{
		/// <summary>
		/// H�ngt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TData">Eingabedatentyp des n�chsten Elementes</typeparam>
		/// <param name="element">Der anzuh�ngende Filter</param>
		void Append(IDataProcessor<TData> element);
	}

	/// <summary>
	/// Interface f�r Elemente, an die ein Filter angeh�ngt werden kann
	/// </summary>
	/// <typeparam name="TData">Der zu verarbeitende Datentyp</typeparam>
	[ContractClassFor(typeof(IAppendable<>))]
	internal abstract class AppendableContract<TData> : IAppendable<TData>
	{
		/// <summary>
		/// H�ngt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TData">Eingabedatentyp des n�chsten Elementes</typeparam>
		/// <param name="element">Der anzuh�ngende Filter</param>
		public void Append(IDataProcessor<TData> element)
		{
			Contract.Requires(element != null, "Anzuh�ngendes Element darf nicht null sein.");
		}
	}
}
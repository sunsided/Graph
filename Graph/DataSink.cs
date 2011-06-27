using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
	/// <summary>
	/// Basisklasse für eine Senke
	/// </summary>
	public abstract class DataSink<TData> : DataProcessor<TData>, ISink<TData>
	{
		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataSink{T}"/>-Klasse.
		/// </summary>
		protected DataSink()
		{
		}

		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataSink{T}"/>-Klasse.
		/// </summary>
		/// <param name="registrationTimeout">Der Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten werden soll.</param>
		/// <param name="inputQueueLength">Die maximale Anzahl an Elementen in der Eingangsqueue.</param>
		protected DataSink([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
			: base(registrationTimeout, inputQueueLength)
		{
			Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
			Contract.Requires(inputQueueLength > 0);
		}
	}
}

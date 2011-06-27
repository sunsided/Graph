using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
	/// <summary>
	/// Basisklasse für ein Datenverarbeitungselement mit Ausgang
	/// </summary>
	public abstract class DataFilter<TInput, TOutput> : DataProcessor<TInput>, IFilter<TInput, TOutput>
	{
		/// <summary>
		/// Die Liste der Ausgabeverarbeiter
		/// </summary>
		private readonly List<IDataInput<TOutput>> _outputList = new List<IDataInput<TOutput>>();

		/// <summary>
		/// Die Anzahl der Ausgabeprozessoren
		/// </summary>
		public int OutputProcessorCount { [Pure] get { return _outputList.Count; } }

		/// <summary>
		/// Liste der zu bedienenden Ausgänge während eines Dispatching-Vorganges
		/// </summary>
		private readonly Queue<IDataInput<TOutput>> _currentOutputs = new Queue<IDataInput<TOutput>>();

		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataFilter{TInput, TOutput}"/>-Klasse.
		/// </summary>
		protected DataFilter()
		{
		}

		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataFilter{TInput, TOutput}"/>-Klasse.
		/// </summary>
		/// <param name="registrationTimeout">Der Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten werden soll.</param>
		/// <param name="inputQueueLength">Die maximale Anzahl an Elementen in der Eingangsqueue.</param>
		protected DataFilter([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
			: base(registrationTimeout, inputQueueLength)
		{
			Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
			Contract.Requires(inputQueueLength > 0);
		}

		/// <summary>
		/// Registriert einen Prozessor für die Ausgabewerte
		/// </summary>
		/// <param name="outputProcessor">Der zu registrierende Prozessor.</param>
		/// <returns><c>true</c>, wenn der Prozessor erfolgreich hinzugefügt wurde, ansonsten <c>false</c></returns>
		public bool AttachOutput(IDataInput<TOutput> outputProcessor)
		{
			//Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_outputList.Count) + 1 == _outputList.Count) ||
			//                  (!Contract.Result<bool>() && Contract.OldValue(_outputList.Count) == _outputList.Count));

			// Element eintüten und Verarbeitung starten lassen
			lock (_outputList)
			{
				if (_outputList.Contains(outputProcessor)) return false;
				_outputList.Add(outputProcessor);
				return true;
			}
		}

		/// <summary>
		/// Registriert einen Prozessor für die Ausgabewerte
		/// </summary>
		/// <param name="outputProcessor">Der zu registrierende Prozessor.</param>
		/// <returns><c>true</c>, wenn der Prozessor erfolgreich hinzugefügt wurde, ansonsten <c>false</c></returns>
		public bool DetachOutput(IDataInput<TOutput> outputProcessor)
		{
			//Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_outputList.Count) - 1 == _outputList.Count) ||
			//                  (!Contract.Result<bool>() && Contract.OldValue(_outputList.Count) == _outputList.Count));

			// Element eintüten und Verarbeitung starten lassen
			lock (_outputList)
			{
				return _outputList.Remove(outputProcessor);
			}
		}

		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="payload">Die zu verarbeitenden Daten</param>
		protected sealed override void ProcessData(TInput payload)
		{
			TOutput outputPayload;
			OnProcessingStateChanged(ProcessingState.Processing);
			bool dispatch = ProcessData(payload, out outputPayload);

			// Verteilen
			if (!dispatch) return;
			lock(_outputList)
			{
				OnProcessingStateChanged(ProcessingState.Dispatching);
				// Prozessoren eintüten
				_currentOutputs.Clear();
				_outputList.ForEach(processor => _currentOutputs.Enqueue(processor));
			}

			// Schleifen bis zum Abwinken
			while (_currentOutputs.Count > 0)
			{
				var processor = _currentOutputs.Dequeue();
				if (!processor.RegisterInput(outputPayload))
				{
					_currentOutputs.Enqueue(processor);
				}
			}
		}

		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="input">Der Eingabewert</param>
		/// <param name="output">Der Ausgabewert</param>
		/// <returns>
		/// <c>true</c>, wenn der Ausgabewert an die Ausgänge weitergereicht - oder <c>false</c>,
		/// wenn das Ergebnis verworfen werden soll.
		/// </returns>
		protected abstract bool ProcessData(TInput input, out TOutput output);
	}
}

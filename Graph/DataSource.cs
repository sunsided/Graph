using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Graph
{
	/// <summary>
	/// Basisklasse für ein Datenverarbeitungselement mit Ausgang
	/// </summary>
	public abstract class DataSource<TData> : DataProcessorBase, ISource<TData>
	{
		/// <summary>
		/// Verarbeitungsmodus
		/// </summary>
		public enum SourceResult
		{
			/// <summary>
			/// Nichts tun
			/// </summary>
			Idle,

			/// <summary>
			/// Daten verarbeiten
			/// </summary>
			Process,

			/// <summary>
			/// Verarbeitung abbrechen
			/// </summary>
			StopProcessing
		}

		/// <summary>
		/// Die Liste der Ausgabeverarbeiter
		/// </summary>
		private readonly List<IDataInput<TData>> _outputList = new List<IDataInput<TData>>();

		/// <summary>
		/// Die Anzahl der Ausgabeprozessoren
		/// </summary>
		public int OutputProcessorCount { [Pure] get { return _outputList.Count; } }

		/// <summary>
		/// Liste der zu bedienenden Ausgänge während eines Dispatching-Vorganges
		/// </summary>
		private readonly Queue<IDataInput<TData>> _currentOutputs = new Queue<IDataInput<TData>>();

		/// <summary>
		/// Die Größe der Ausgabequeue
		/// </summary>
		private readonly Queue<TData> _outputQueue = new Queue<TData>();

		/// <summary>
		/// Der Verarbeitungstask
		/// </summary>
		private readonly Task _processingTask;

		/// <summary>
		/// Der Ausgabetask
		/// </summary>
		private readonly Task _outputTask;

		/// <summary>
		/// Gibt an, ob die Verarbeitung angehalten werden soll
		/// </summary>
		private volatile bool _stopProcessing;

		/// <summary>
		/// Vorgabewert für <see cref="OutputQueueLength"/>
		/// </summary>
		internal const int OutputQueueLengthDefault = 100;

		/// <summary>
		/// Der Semaphor, der den Zugriff auf die Ausgabequeue regelt.
		/// <seealso cref="_outputQueue"/>
		/// </summary>
		private readonly Semaphore _outputQueueSemaphore;

		/// <summary>
		/// Die Länge der Eingabequeue
		/// </summary>
		public int OutputQueueLength { [Pure] get; private set; }

		/// <summary>
		/// Threadsynchronisierungsobjekt, das den Ausgabeloop startet
		/// </summary>
		private readonly AutoResetEvent _outputStartTrigger = new AutoResetEvent(false);

		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataSource{TData}"/>-Klasse.
		/// </summary>
		protected DataSource([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
		{
			Contract.Requires(outputQueueLength > 0);

			OutputQueueLength = outputQueueLength;
			_outputQueueSemaphore = new Semaphore(0, outputQueueLength);

			_processingTask = new Task(ProcessingLoop, TaskCreationOptions.LongRunning);
			_outputTask = new Task(OutputLoop, TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Registriert einen Prozessor für die Ausgabewerte
		/// </summary>
		/// <param name="outputProcessor">Der zu registrierende Prozessor.</param>
		/// <returns><c>true</c>, wenn der Prozessor erfolgreich hinzugefügt wurde, ansonsten <c>false</c></returns>
		public bool AttachOutput(IDataInput<TData> outputProcessor)
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
		public bool DetachOutput(IDataInput<TData> outputProcessor)
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
		/// Beginnt die Verarbeitung
		/// </summary>
		public override void StartProcessing()
		{
			if (_processingTask.Status == TaskStatus.WaitingToRun || _processingTask.Status == TaskStatus.Running) return;
			_processingTask.Start();
		}

		/// <summary>
		/// Hält die Verarbeitung an
		/// </summary>
		public override void StopProcessing()
		{
			Contract.Ensures(_stopProcessing == true);
			_stopProcessing = true;
			_outputStartTrigger.Set();
		}

		/// <summary>
		/// Erzeugt Daten in einer Endlosschleife
		/// <seealso cref="StartProcessing"/>
		/// <seealso cref="StopProcessing"/>
		/// </summary>
		private void ProcessingLoop()
		{
			do
			{
				TData payload;
				SourceResult result = CreateData(out payload);
				if (result == SourceResult.Idle)
				{
					Thread.Sleep(0);
					if (_stopProcessing) break;
					continue;
				}
				if(result == SourceResult.StopProcessing)
				{
					break;
				}

				// Eintüten
				_outputQueueSemaphore.WaitOne(); // TODO: Timeout!
				_outputQueue.Enqueue(payload);
				_outputStartTrigger.Set();

			} while (!_stopProcessing);
		}

		/// <summary>
		/// Der Ausgabeloop
		/// </summary>
		private void OutputLoop()
		{
			while (!_stopProcessing)
			{
				_outputStartTrigger.WaitOne();
				OnProcessingStateChanged(ProcessingState.Dispatching);

				// Verteilen, solange Daten vorhanden sind
				int count;
				lock (_outputQueue) count = _outputQueue.Count;
				while (!_stopProcessing && count-- > 0)
				{
					// Ausgabewert beziehen
					TData outputPayload;
					lock (_outputQueue)
					{
						outputPayload = _outputQueue.Dequeue();
						_outputQueueSemaphore.Release(1);
					}

					// Ausgänge laden
					lock (_outputList)
					{
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
			}
		}

		/// <summary>
		/// Erzeugt die Daten
		/// </summary>
		/// <param name="payload">Die auszugebenden Daten</param>
		/// <returns>
		/// <see cref="SourceResult.Process"/>, wenn die Verarbeitung fortgesetzt werden soll, <see cref="SourceResult.StopProcessing"/>, wenn die 
		/// Ausgabe (<paramref name="payload"/>) verworfen und die Verarbeitung abgebrochen werden soll oder <see cref="SourceResult.Idle"/>, wenn
		/// nichts geschehen soll.
		/// </returns>
		protected abstract SourceResult CreateData(out TData payload);
	}
}

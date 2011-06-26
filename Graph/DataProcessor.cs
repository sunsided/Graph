using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Graph
{
	/// <summary>
	/// Basisklasse für ein Datenverarbeitungselement
	/// </summary>
	public abstract class DataProcessor<TData>
	{
		/// <summary>
		/// Vorgabewert für <see cref="_registrationTimeout"/>.
		/// </summary>
		private const int RegistrationTimeoutDefault = Timeout.Infinite;

		/// <summary>
		/// Vorgabewert für <see cref="InputQueueLength"/>
		/// </summary>
		private const int InputQueueLengthDefault = 100;

		/// <summary>
		/// Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten
		/// werden soll.
		/// <seealso cref="Register"/>
		/// </summary>
		private readonly int _registrationTimeout;

		/// <summary>
		/// Timeout in Millisekunden, der beim warten auf registrierte Elemente eingehalten
		/// werden soll.
		/// <seealso cref="Register"/>
		/// </summary>
		private const int ProcessingLockTimeout = Timeout.Infinite;

		/// <summary>
		/// Die Queue der Eingabewerte
		/// </summary>
		private readonly Queue<TData> _inputQueue = new Queue<TData>();

		/// <summary>
		/// Der Semaphor, der den Zugriff auf die Eingabequeue regelt.
		/// <seealso cref="Register"/>
		/// <seealso cref="_inputQueue"/>
		/// </summary>
		private readonly Semaphore _inputQueueSemaphore;

		/// <summary>
		/// Die Länge der Eingabequeue
		/// </summary>
		public int InputQueueLength { [Pure] get; private set; }

		/// <summary>
		/// Threadsynchronisierungsobjekt, das den Verarbeitungsloop startet
		/// </summary>
		private readonly AutoResetEvent _processStartTrigger = new AutoResetEvent(false);

		/// <summary>
		/// Gibt an, ob die Verarbeitung angehalten werden soll
		/// </summary>
		private volatile bool _stopProcessing;

		/// <summary>
		/// Liest oder setzt das benutzerdefinierte Tag
		/// </summary>
		public object Tag { [Pure] get; set; }

		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataProcessor{T}"/>-Klasse.
		/// </summary>
		protected DataProcessor()
			: this(RegistrationTimeoutDefault, InputQueueLengthDefault)
		{
		}

		/// <summary>
		/// Erzeugt eine neue Instanz der <see cref="DataProcessor{T}"/>-Klasse.
		/// </summary>
		/// <param name="registrationTimeout">Der Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten werden soll.</param>
		/// <param name="inputQueueLength">Die maximale Anzahl an Elementen in der Eingangsqueue.</param>
		protected DataProcessor([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
		{
			Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
			Contract.Requires(inputQueueLength > 0);

			_registrationTimeout = registrationTimeout;
			InputQueueLength = inputQueueLength;
			_inputQueueSemaphore = new Semaphore(0, inputQueueLength);
		}

		/// <summary>
		/// Registriert einen Eingabewert
		/// <para>
		/// Wenn die Eingabequeue frei ist, ist dieser Aufruf nicht blockierend. 
		/// Ist die Queue voll, blockiert der Aufruf so lange, bis neue Werte 
		/// nachgereicht werden können.
		/// </para>
		/// </summary>
		/// <param name="input">Der zu registrierende Eingabewert.</param>
		public bool Register(TData input)
		{
			Contract.Ensures((Contract.Result<bool>() && Contract.OldValue(_inputQueue.Count) == _inputQueue.Count + 1) ||
			                  (!Contract.Result<bool>() && Contract.OldValue(_inputQueue.Count) == _inputQueue.Count));

			// Warten, bis ein Eingabeslot frei wird
			if (!_inputQueueSemaphore.WaitOne(_registrationTimeout)) return false;

			// Element eintüten und Verarbeitung starten lassen
			lock (_inputQueue) _inputQueue.Enqueue(input);
			_processStartTrigger.Set();
			return true;
		}

		/// <summary>
		/// Verarbeitet die Daten in der Eingangsqueue, bis <see cref="_stopProcessing"/> auf 
		/// <c>true</c> gesetzt wird.
		/// <para>
		/// Sind keine weiteren Daten in der Eingangsqueue, blockiert die Methode, bis
		/// <see cref="_processStartTrigger"/> gesetzt wird.
		/// </para>
		/// <seealso cref="Register(TData)"/>
		/// <seealso cref="StopProcessing"/>
		/// </summary>
		private void ProcessingLoop()
		{
			Queue<TData> processingQueue = new Queue<TData>();
			while (!_stopProcessing)
			{
				// Auf Start warten
				if (!_processStartTrigger.WaitOne(ProcessingLockTimeout)) continue;

				// Daten beziehen
				lock (_inputQueue)
				{
					if (_inputQueue.Count == 0) continue;
					while(_inputQueue.Count > 0)
					{
						processingQueue.Enqueue(_inputQueue.Dequeue());
					}
				}

				// Daten verarbeiten
				while(processingQueue.Count > 0)
				{
					TData payload = processingQueue.Dequeue();
					ProcessDataInternal(payload);
				}
			}
		}

		/// <summary>
		/// Hält die Verarbeitung an
		/// </summary>
		public void StopProcessing()
		{
			Contract.Ensures(_stopProcessing == true);
			_stopProcessing = true;
			_processStartTrigger.Set();
		}

		/// <summary>
		/// Verarbeitet den gegebenen Datensatz
		/// </summary>
		/// <param name="payload">Zu verarbeitender Datensatz</param>
		private void ProcessDataInternal(TData payload)
		{
			try
			{
				
			}
			catch(Exception e)
			{
				throw;
			}
		}
	}
}

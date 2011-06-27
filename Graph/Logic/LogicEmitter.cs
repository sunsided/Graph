using System.Collections.Generic;
using System.Threading;

namespace Graph.Logic
{
	/// <summary>
	/// Quelle, die Wahrweitswerte emittiert
	/// </summary>
	public sealed class LogicEmitter : DataSource<bool>
	{
		/// <summary>
		/// WaitHandle, das das Erzeugen von Daten regelt
		/// </summary>
		private readonly AutoResetEvent _starter = new AutoResetEvent(false);

		/// <summary>
		/// Timeout in Millisekunden
		/// </summary>
		private const int StarterTimeoutMs = 1000;

		/// <summary>
		/// Emissionsqueue
		/// </summary>
		private readonly Queue<bool> _emissionQueue = new Queue<bool>();

		/// <summary>
		/// Erzeugt die Daten
		/// </summary>
		/// <param name="payload">Die auszugebenden Daten</param>
		/// <returns>
		/// <see cref="DataSource{TData}.SourceResult.Process"/>, wenn die Verarbeitung fortgesetzt werden soll, <see cref="DataSource{TData}.SourceResult.StopProcessing"/>, wenn die 
		/// Ausgabe (<paramref name="payload"/>) verworfen und die Verarbeitung abgebrochen werden soll oder <see cref="DataSource{TData}.SourceResult.Idle"/>, wenn
		/// nichts geschehen soll.
		/// </returns>
		protected override SourceResult CreateData(out bool payload)
		{
			lock (_emissionQueue)
			{
				if (_emissionQueue.Count > 0)
				{
					payload = _emissionQueue.Dequeue();
					return SourceResult.Process;
				}
			}
			_starter.WaitOne(StarterTimeoutMs);
			payload = false; // Don't care
			return SourceResult.Idle;
		}

		/// <summary>
		/// Emittiert den Wert <c>true</c>.
		/// <para>Ein Aufruf von <see cref="DataSource{T}.StartProcessing"/> ist notwendig, damit die Verarbeitung gestartet wird.</para>
		/// </summary>
		public void EmitTrue()
		{
			Emit(true);
		}

		/// <summary>
		/// Emittiert den Wert <c>false</c>.
		/// <para>Ein Aufruf von <see cref="DataSource{T}.StartProcessing"/> ist notwendig, damit die Verarbeitung gestartet wird.</para>
		/// </summary>
		public void EmitFalse()
		{
			Emit(false);
		}

		/// <summary>
		/// Emittiert den angegebenen Wert.
		/// <para>Ein Aufruf von <see cref="DataSource{T}.StartProcessing"/> ist notwendig, damit die Verarbeitung gestartet wird.</para>
		/// </summary>
		/// <param name="value">Der zu emittierende Wert</param>
		public void Emit(bool value)
		{
			lock(_emissionQueue)
			{
				_emissionQueue.Enqueue(value);
				_starter.Set();
			}
		}
	}
}

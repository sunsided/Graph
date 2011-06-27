using System.Collections.Generic;
using System.ComponentModel;

namespace Graph
{
	/// <summary>
	/// Filter mit zwei Eingängen
	/// </summary>
	/// <typeparam name="TInput1">Erster Eingangsdatentyp</typeparam>
	/// <typeparam name="TInput2">Zweiter Eingangsdatentyp</typeparam>
	/// <typeparam name="TOutput">Ausgangsdatentyp</typeparam>
	public abstract class DualInFilter<TInput1, TInput2, TOutput> : DataSource<TOutput>, IDualInFilter<TInput1, TInput2, TOutput>
	{
		/// <summary>
		/// Senke, die Daten an die Elternqueue durchreicht
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private sealed class PassthroughSink<T> : DataSink<T>
		{
			/// <summary>
			/// Die zu verwendende Queue
			/// </summary>
			public Queue<T> Queue { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;.PassthroughSink&lt;T&gt;"/> class.
			/// </summary>
			/// <param name="queue">The queue.</param>
			/// <remarks></remarks>
			public PassthroughSink(Queue<T> queue)
			{
				Queue = queue;
			}

			/// <summary>
			/// Verarbeitet die Daten
			/// </summary>
			/// <param name="payload">Die zu verarbeitenden Daten</param>
			protected override void ProcessData(T payload)
			{
				Queue.Enqueue(payload); // TODO: Semaphore beachten und Rückmeldung geben
			}
		}

		/// <summary>
		/// Erster Input
		/// </summary>
		private readonly Queue<TInput1> _input1 = new Queue<TInput1>(); // TODO Limitierungs-Semaphor

		/// <summary>
		/// Zweiter Input
		/// </summary>
		private readonly Queue<TInput2> _input2 = new Queue<TInput2>(); // TODO Limitierungs-Semaphor

		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <remarks></remarks>
		protected DualInFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <param name="outputQueueLength">Length of the output queue.</param>
		/// <remarks></remarks>
		protected DualInFilter([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength) : base(outputQueueLength)
		{
			_inputSink1 = new PassthroughSink<TInput1>(_input1);
			_inputSink2 = new PassthroughSink<TInput2>(_input2);
		}

		/// <summary>
		/// Erzeugt die Daten
		/// </summary>
		/// <param name="payload">Die auszugebenden Daten</param>
		/// <returns>
		/// <see cref="DataSource{TData}.SourceResult.Process"/>, wenn die Verarbeitung fortgesetzt werden soll, <see cref="DataSource{TData}.SourceResult.StopProcessing"/>, wenn die 
		/// Ausgabe (<paramref name="payload"/>) verworfen und die Verarbeitung abgebrochen werden soll oder <see cref="DataSource{TData}.SourceResult.Idle"/>, wenn
		/// nichts geschehen soll.
		/// </returns>
		protected override SourceResult CreateData(out TOutput payload)
		{
			TInput1 value1 = _input1.Dequeue();
			TInput2 value2 = _input2.Dequeue();

			// Weiterverarbeiten
			TOutput output;
			if (ProcessData(value1, value2, out output))
			{
				payload = output;
				return SourceResult.Process;
			}

			// Nichts tun
			payload = default(TOutput);
			return SourceResult.Idle;
		}

		/// <summary>
		/// Starts the processing.
		/// </summary>
		/// <remarks></remarks>
		public override void StartProcessing()
		{
			_inputSink1.StartProcessing();
			_inputSink2.StartProcessing();
			base.StartProcessing();
		}

		/// <summary>
		/// Stops the processing.
		/// </summary>
		/// <remarks></remarks>
		public override void StopProcessing()
		{
			_inputSink1.StopProcessing();
			_inputSink2.StopProcessing();
			base.StopProcessing();
		}

		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="input1">Der erste Eingabewert</param>
		/// <param name="input2">Der zweite Eingabewert</param>
		/// <param name="output">Der Ausgabewert</param>
		/// <returns>
		/// <c>true</c>, wenn der Ausgabewert an die Ausgänge weitergereicht - oder <c>false</c>,
		/// wenn das Ergebnis verworfen werden soll.
		/// </returns>
		protected abstract bool ProcessData(TInput1 input1, TInput2 input2, out TOutput output);

		/// <summary>
		/// Der erste Input
		/// </summary>
		private readonly PassthroughSink<TInput1> _inputSink1;

		/// <summary>
		/// Der erste Input
		/// </summary>
		public ISink<TInput1> Input1
		{
			get { return _inputSink1; }
		}

		/// <summary>
		/// Der zweite Input
		/// </summary>
		private readonly PassthroughSink<TInput2> _inputSink2;

		/// <summary>
		/// Der zweite Input
		/// </summary>
		public ISink<TInput2> Input2
		{
			get { return _inputSink2; }
		}
	}

	/// <summary>
	/// Filter mit zwei Eingängen
	/// </summary>
	/// <typeparam name="TInput">Eingangsdatentyp</typeparam>
	/// <typeparam name="TOutput">Ausgangsdatentyp</typeparam>
	public abstract class DualInFilter<TInput, TOutput> : DualInFilter<TInput, TInput, TOutput>, IDualInFilter<TInput, TOutput>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <remarks></remarks>
		protected DualInFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <param name="outputQueueLength">Length of the output queue.</param>
		/// <remarks></remarks>
		protected DualInFilter([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
			: base(outputQueueLength)
		{
		}
	}
}

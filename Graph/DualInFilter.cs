using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;

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
			private Queue<T> Queue { get; set; }

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
			/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;.PassthroughSink&lt;T&gt;"/> class.
			/// </summary>
			/// <param name="queue">The queue.</param>
			/// <param name="registrationTimeout">Der Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten werden soll.</param>
			/// <param name="inputQueueLength">Die maximale Anzahl an Elementen in der Eingangsqueue.</param>
			/// <remarks></remarks>
			public PassthroughSink(Queue<T> queue, [DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength)
				: base(registrationTimeout, inputQueueLength)
			{
				Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
				Contract.Requires(inputQueueLength > 0);
				Queue = queue;
			}

			/// <summary>
			/// Verarbeitet die Daten
			/// </summary>
			/// <param name="payload">Die zu verarbeitenden Daten</param>
			protected override void ProcessData(T payload)
			{
				// Semaphor-Locking wird hier nicht benötigt, da dies die Basisklasse für uns übernimmt
				Queue.Enqueue(payload);
			}
		}

		/// <summary>
		/// Vorgabewert für den Registrierungstimeout
		/// </summary>
		internal const int RegistrationTimeoutDefault = Timeout.Infinite;

		/// <summary>
		/// Vorgabewert für die Länge der Eingabequeue
		/// </summary>
		internal const int InputQueueLengthDefault = 100;
		
		/// <summary>
		/// Erster Input
		/// </summary>
		private readonly Queue<TInput1> _input1 = new Queue<TInput1>();

		/// <summary>
		/// Zweiter Input
		/// </summary>
		private readonly Queue<TInput2> _input2 = new Queue<TInput2>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <remarks></remarks>
		protected DualInFilter()
		{
			Contract.Ensures(_inputSink1 != null && _inputSink2 != null);

			_inputSink1 = new PassthroughSink<TInput1>(_input1);
			_inputSink2 = new PassthroughSink<TInput2>(_input2);
			StartProcessing();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <param name="outputQueueLength">Length of the output queue.</param>
		/// <remarks></remarks>
		protected DualInFilter([DefaultValue(OutputQueueLengthDefault)] int outputQueueLength)
			: base(outputQueueLength)
		{
			Contract.Requires(outputQueueLength > 0);
			Contract.Ensures(_inputSink1 != null && _inputSink2 != null);
			Contract.Ensures(Input1 == _inputSink1);
			Contract.Ensures(Input2 == _inputSink2);

			_inputSink1 = new PassthroughSink<TInput1>(_input1);
			_inputSink2 = new PassthroughSink<TInput2>(_input2);
			StartProcessing();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DualInFilter&lt;TInput1, TInput2, TOutput&gt;"/> class.
		/// </summary>
		/// <param name="outputQueueLength">Length of the output queue.</param>
		/// <param name="registrationTimeout">Der Timeout in Millisekunden, der beim Registrieren von Elementen eingehalten werden soll.</param>
		/// <param name="inputQueueLength">Die maximale Anzahl an Elementen in der Eingangsqueue.</param>
		/// <remarks></remarks>
		protected DualInFilter([DefaultValue(RegistrationTimeoutDefault)] int registrationTimeout, [DefaultValue(InputQueueLengthDefault)] int inputQueueLength, [DefaultValue(OutputQueueLengthDefault)] int outputQueueLength) 
			: base(outputQueueLength)
		{
			Contract.Requires(registrationTimeout == Timeout.Infinite || registrationTimeout > 0);
			Contract.Requires(inputQueueLength > 0);
			Contract.Requires(outputQueueLength > 0);
			Contract.Ensures(_inputSink1 != null && _inputSink2 != null);
			Contract.Ensures(Input1 == _inputSink1);
			Contract.Ensures(Input2 == _inputSink2);

			_inputSink1 = new PassthroughSink<TInput1>(_input1, registrationTimeout, inputQueueLength);
			_inputSink2 = new PassthroughSink<TInput2>(_input2, registrationTimeout, inputQueueLength);
			StartProcessing();
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
			payload = default(TOutput);
			if (_input1.Count == 0) return SourceResult.Idle;
			if (_input2.Count == 0) return SourceResult.Idle;

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
		public sealed override void StartProcessing()
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
			[Pure] get
			{
				Contract.Ensures(Contract.Result<ISink<TInput1>>() != null);
				return _inputSink1;
			}
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
			[Pure] get
			{
				Contract.Ensures(Contract.Result<ISink<TInput1>>() != null);
				return _inputSink2;
			}
		}

		/// <summary>
		/// Invariante für den Vertrag
		/// </summary>
		/// <remarks></remarks>
		[ContractInvariantMethod]
		private void ObjectInvariant()
		{
			Contract.Invariant(_inputSink1 != null);
			Contract.Invariant(_inputSink2 != null);
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
			Contract.Requires(outputQueueLength > 0);
		}
	}
}

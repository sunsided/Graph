namespace Graph
{
	/// <summary>
	/// Quelle, die immer denselben Wert liefert
	/// </summary>
	/// <typeparam name="T">Der Ausgangsdatentyp</typeparam>
	public sealed class ConstantSource<T> : ElementBase<T>, ISource<T>
	{
		/// <summary>
		/// Der zu erzeugende Wert
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstantSource&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public ConstantSource(T value)
		{
			Value = value;
		}

		/// <summary>
		/// Erzeugt die Eingabe
		/// </summary>
		/// <rereturns>Die Ausgabedaten</rereturns>
		public T Create()
		{
			return Value;
		}

		/// <summary>
		/// Erzeugt die Ausgabe und leitet sie an das nächste Element weiter
		/// </summary>
		public void Process()
		{
			SetProcessingState(ProcessState.Filtering, null);
			T value = Create();
			SetProcessingState(ProcessState.Dispatching, value);
			Follower.Process(value);
			SetProcessingState(ProcessState.Idle, null);
		}
	}
}

using System;

namespace Graph
{
	/// <summary>
	/// Quelle, die einen pseudozufälligen <see cref="Int32"/>-Wert in einem bestimmten Bereich liefert
	/// </summary>
	/// <typeparam name="T">Der Ausgangsdatentyp</typeparam>
	public class RandomIntegerSource : ElementBase<int>, ISource<int>
	{
		/// <summary>
		/// Der zu erzeugende Wert
		/// </summary>
		public int Minimum { get; set; }

		/// <summary>
		/// Der zu erzeugende Wert
		/// </summary>
		public int Maximum { get; set; }

		/// <summary>
		/// Der Randomizer
		/// </summary>
		private readonly Random _random = new Random();

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIntegerSource"/> class.
		/// </summary>
		/// <param name="min">Minimalwert</param>
		/// <param name="max">Maximalwert</param>
		public RandomIntegerSource(int min, int max)
		{
			Minimum = min;
			Maximum = max;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomIntegerSource"/> class.
		/// </summary>
		/// <param name="min">Minimalwert</param>
		/// <param name="max">Maximalwert</param>
		/// <param name="seed">Der zu verwendende Random-Seed</param>
		public RandomIntegerSource(int min, int max, int seed)
			: this(min, max)
		{
			_random = new Random(seed);
		}

		/// <summary>
		/// Erzeugt die Eingabe
		/// </summary>
		/// <rereturns>Die Ausgabedaten</rereturns>
		public int Create()
		{
			return _random.Next(Minimum, Maximum);
		}

		/// <summary>
		/// Erzeugt die Ausgabe und leitet sie an das nächste Element weiter
		/// </summary>
		public void Process()
		{
			SetProcessingState(ProcessState.Filtering, null);
			int value = Create();
			SetProcessingState(ProcessState.Dispatching, value);
			Follower.Process(value);
			SetProcessingState(ProcessState.Idle, null);
		}
	}
}

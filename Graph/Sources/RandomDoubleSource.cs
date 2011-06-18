using System;

namespace Graph.Sources
{
	/// <summary>
	/// Quelle, die einen pseudozufälligen <see cref="Double"/>-Wert in einem bestimmten Bereich liefert
	/// </summary>
	public class RandomDoubleSource : ElementBase<double>, ISource<double>
	{
		/// <summary>
		/// Der zu erzeugende Wert
		/// </summary>
		public double Minimum { get; set; }

		/// <summary>
		/// Der zu erzeugende Wert
		/// </summary>
		public double Maximum { get; set; }

		/// <summary>
		/// Der Randomizer
		/// </summary>
		private readonly Random _random = new Random();

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomDoubleSource"/> class.
		/// </summary>
		/// <param name="min">Minimalwert</param>
		/// <param name="max">Maximalwert</param>
		public RandomDoubleSource(double min, double max)
		{
			Minimum = min;
			Maximum = max;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomDoubleSource"/> class.
		/// </summary>
		/// <param name="min">Minimalwert</param>
		/// <param name="max">Maximalwert</param>
		/// <param name="seed">Der zu verwendende Random-Seed</param>
		public RandomDoubleSource(double min, double max, int seed)
			: this(min, max)
		{
			_random = new Random(seed);
		}

		/// <summary>
		/// Erzeugt die Eingabe
		/// </summary>
		/// <rereturns>Die Ausgabedaten</rereturns>
		public double Create()
		{
			return _random.NextDouble()*(Maximum - Minimum) + Minimum;
		}

		/// <summary>
		/// Erzeugt die Ausgabe und leitet sie an das nächste Element weiter
		/// </summary>
		public void Process()
		{
			SetProcessingState(ProcessState.Filtering);
			double value = Create();
			SetProcessingState(ProcessState.Dispatching);
			Follower.Process(value);
			SetProcessingState(ProcessState.Idle);
		}
	}
}

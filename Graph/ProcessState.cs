namespace Graph
{
	/// <summary>
	/// Zustand des Elementes
	/// </summary>
	public enum ProcessState
	{
		/// <summary>
		/// Tut nichts
		/// </summary>
		Idle,

		/// <summary>
		/// Erzeugt oder filtert Daten
		/// </summary>
		Filtering,

		/// <summary>
		/// Reicht die Daten weiter
		/// </summary>
		Dispatching
	}
}

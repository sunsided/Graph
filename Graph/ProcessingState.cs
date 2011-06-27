namespace Graph
{
	/// <summary>
	/// Datenverarbeitungszustand
	/// </summary>
	public enum ProcessingState
	{
		/// <summary>
		/// Angehalten
		/// </summary>
		Stopped,

		/// <summary>
		/// Wartend
		/// </summary>
		Idle,

		/// <summary>
		/// Vorbereitung der Daten
		/// </summary>
		Preparing,

		/// <summary>
		/// Verarbeitung der Daten
		/// </summary>
		Processing,

		/// <summary>
		/// Verteilen der Daten
		/// </summary>
		Dispatching
	}
}

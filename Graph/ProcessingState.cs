namespace Graph
{
    /// <summary>
    /// Data processing state
    /// </summary>
    public enum ProcessingState
    {
        /// <summary>
        /// Stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Idling
        /// </summary>
        Idle,

        /// <summary>
        /// Preparing data from input
        /// </summary>
        Preparing,

        /// <summary>
        /// Processing data
        /// </summary>
        Processing,

        /// <summary>
        /// Dispatching data to outputs
        /// </summary>
        Dispatching
    }
}

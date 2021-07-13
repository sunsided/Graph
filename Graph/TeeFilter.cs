namespace Graph
{
    /// <summary>
    /// Basisklasse für ein Datenverarbeitungselement mit Ausgang
    /// </summary>
    public sealed class TeeFilter<TData> : DataFilter<TData, TData>
    {
        /// <summary>
        /// Verteilt den Eingang aug die Ausgänge
        /// </summary>
        /// <param name="input">Der Eingabewert</param>
        /// <param name="output">Der Ausgabewert</param>
        /// <returns>Immer <c>true</c>.
        /// </returns>
        protected override bool ProcessData(TData input, out TData output)
        {
            output = input;
            return true;
        }
    }
}

using System;

namespace Graph
{
	/// <summary>
	/// Interface für ein Element, das seinen Beschäftigungsstatus melden kann
	/// </summary>
	public interface IProcessIndicator
	{
		/// <summary>
		/// Der Prozesszustand hat sich geändert
		/// </summary>
		event EventHandler<ProcessStateEventArgs> StateChanged;
	}
}

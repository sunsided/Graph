﻿using System;

namespace Graph
{
	/// <summary>
	/// Interface für ein Element, das seinen Beschäftigungsstatus melden kann
	/// </summary>
	public interface IProcessIndicator
	{
		/// <summary>
		/// Ermittelt den Zustand des Elementes
		/// </summary>
		ProcessState State { get; }

		/// <summary>
		/// Der Prozesszustand hat sich geändert
		/// </summary>
		event EventHandler<ProcessStateEventArgs> StateChanged;
	}
}
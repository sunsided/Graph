using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph.Logic
{
	/// <summary>
	/// Sink that receives a boolean value and invokes an action
	/// </summary>
	public sealed class LogicActionInvoker : ActionInvoker<bool>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LogicActionInvoker"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		public LogicActionInvoker(Action<bool> action)
			: base(action)
		{
			Contract.Requires(action != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		public LogicActionInvoker(Action<bool> action, TaskScheduler scheduler)
			: base(action, scheduler)
		{
			Contract.Requires(action != null);
			Contract.Requires(scheduler != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <param name="taskCreationOptions">Die Task-Erzeugungsoptionen</param>
		public LogicActionInvoker(Action<bool> action, TaskScheduler scheduler, TaskCreationOptions taskCreationOptions)
			: base(action, scheduler, taskCreationOptions)
		{
			Contract.Requires(action != null);
			Contract.Requires(scheduler != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		/// <param name="taskCreationOptions">Die Task-Erzeugungsoptionen</param>
		public LogicActionInvoker(Action<bool> action, TaskCreationOptions taskCreationOptions)
			: base(action, taskCreationOptions)
		{
			Contract.Requires(action != null);
		}
	}
}

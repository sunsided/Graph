using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph
{
	/// <summary>
	/// Senke, die eine Aktion ausführt und den eingehenden Wert übergibt
	/// </summary>
	public class ActionInvoker<TData> : DataSink<TData>
	{
		/// <summary>
		/// Die auszuführende Aktion
		/// </summary>
		private readonly Action<TData> _action;

		/// <summary>
		/// Der zu verwendende Task-Scheduler
		/// </summary>
		private readonly TaskScheduler _scheduler;

		/// <summary>
		/// Der zu verwendende Task-Scheduler
		/// </summary>
		private readonly TaskCreationOptions _creationOptions = TaskCreationOptions.None;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		public ActionInvoker(Action<TData> action)
		{
			Contract.Requires(action != null);
			_action = action;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		public ActionInvoker(Action<TData> action, TaskScheduler scheduler)
			: this(action)
		{
			Contract.Requires(action != null);
			Contract.Requires(scheduler != null);
			_scheduler = scheduler;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <param name="taskCreationOptions">Die Task-Erzeugungsoptionen</param>
		public ActionInvoker(Action<TData> action, TaskScheduler scheduler, TaskCreationOptions taskCreationOptions)
			: this(action, scheduler)
		{
			Contract.Requires(action != null);
			Contract.Requires(scheduler != null);
			_scheduler = scheduler;
			_creationOptions = taskCreationOptions;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionInvoker{T}"/> class.
		/// </summary>
		/// <param name="action">The auszuführende Aktion.</param>
		/// <param name="taskCreationOptions">Die Task-Erzeugungsoptionen</param>
		public ActionInvoker(Action<TData> action, TaskCreationOptions taskCreationOptions)
			: this(action)
		{
			Contract.Requires(action != null);
			_creationOptions = taskCreationOptions;
		}

		/// <summary>
		/// Verarbeitet die Daten
		/// </summary>
		/// <param name="payload">Die zu verarbeitenden Daten</param>
		protected override void ProcessData(TData payload)
		{
			Task task = new Task(() => _action(payload), _creationOptions);
			if (_scheduler == null) task.Start(); else task.Start(_scheduler);
		}
	}
}

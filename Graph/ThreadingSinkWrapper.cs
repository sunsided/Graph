using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph
{
	/// <summary>
	/// Filter, welches die Weiterverarbeitung in einem neuen Thread durchführt.
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class ThreadingSinkWrapper<T> : StateBase, ISink<T>
	{
		/// <summary>
		/// Der zu verwendende Task Scheduler
		/// </summary>
		private readonly TaskScheduler _scheduler;

		/// <summary>
		/// Die zu verwendenden Task Creation Options
		/// </summary>
		private readonly TaskCreationOptions _options;

		/// <summary>
		/// Die Datensenke
		/// </summary>
		private readonly ISink<T> _sink;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="sink">Das nachfolgende Element</param>
		public ThreadingSinkWrapper(ISink<T> sink)
		{
			Contract.Requires(sink != null);
			Contract.Assume(sink != this);
			
			_sink = sink;
			_sink.StateChanged += (sender, args) => SetProcessingState(args.State, args.Input);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="sink">Das nachfolgende Element</param>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		public ThreadingSinkWrapper(ISink<T> sink, TaskScheduler scheduler)
			: this(sink, scheduler, TaskCreationOptions.None)
		{
			Contract.Requires(sink != null);
			Contract.Requires(scheduler != null);
			Contract.Assume(sink != this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="sink">Das nachfolgende Element</param>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		/// <param name="options">Die zu verwendenden Task-Erzeugungsoptionen</param>
		public ThreadingSinkWrapper(ISink<T> sink, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(sink != null);
			Contract.Requires(scheduler != null);
			Contract.Assume(sink != this);

			_sink = sink;
			_scheduler = scheduler;
			_options = options;

			_sink.StateChanged += (sender, args) => SetProcessingState(args.State, args.Input);
		}

		/// <summary>
		/// Erzeugt Daten
		/// </summary>
		public void Process(T input)
		{
			// Neuen Task erzeugen
			Action action = delegate { _sink.Process(input); };
			Task task = new Task(action, _options);

			// Task starten
			if (_scheduler != null)
			{
				task.Start(_scheduler);
			}
			else
			{
				task.Start();
			}

			// Fertig
			SetProcessingState(ProcessState.Idle, null);
		}
	}

	/// <summary>
	/// Helfermethoden für <see cref="ThreadingSinkWrapper{T}"/>
	/// </summary>
	public static class ThreadingSinkHelper
	{
		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="sink">Der zu wrappende Processor</param>
		/// <returns>Eine Threading-Senke</returns>
		public static ThreadingSinkWrapper<T> MakeThreaded<T>(this ISink<T> sink)
		{
			Contract.Requires(sink != null);
			Contract.Requires(!(sink is ThreadingSinkWrapper<T>));
			Contract.Ensures(Contract.Result<ThreadingSinkWrapper<T>>() != null);
			return new ThreadingSinkWrapper<T>(sink);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="sink">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <returns>Eine Threading-Senke</returns>
		public static ThreadingSinkWrapper<T> MakeThreaded<T>(this ISink<T> sink, TaskScheduler scheduler)
		{
			Contract.Requires(sink != null);
			Contract.Requires(scheduler != null);
			Contract.Requires(!(sink is ThreadingSinkWrapper<T>));
			Contract.Ensures(Contract.Result<ThreadingSinkWrapper<T>>() != null);
			return new ThreadingSinkWrapper<T>(sink, scheduler);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="sink">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <param name="options">Die Task-Erzeugungsoptionen</param>
		/// <returns>Eine Threading-Senke</returns>
		public static ThreadingSinkWrapper<T> MakeThreaded<T>(this ISink<T> sink, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(sink != null);
			Contract.Requires(scheduler != null);
			Contract.Requires(!(sink is ThreadingSinkWrapper<T>));
			Contract.Ensures(Contract.Result<ThreadingSinkWrapper<T>>() != null);
			return new ThreadingSinkWrapper<T>(sink, scheduler, options);
		}
	}
}

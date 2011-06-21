using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph
{
	/// <summary>
	/// Filter, welches die Weiterverarbeitung in einem neuen Thread durchführt.
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class ThreadingFilter<T> : PassthroughFilter<T>
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
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TInt&gt;"/> class.
		/// </summary>
		public ThreadingFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TInt&gt;"/> class.
		/// </summary>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		public ThreadingFilter(TaskScheduler scheduler)
			: this(scheduler, TaskCreationOptions.None)
		{
			Contract.Requires(scheduler != null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TInt&gt;"/> class.
		/// </summary>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		/// <param name="options">Die zu verwendenden Task-Erzeugungsoptionen</param>
		public ThreadingFilter(TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(scheduler != null);
			_scheduler = scheduler;
			_options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		public ThreadingFilter(IDataProcessor<T> next)
		{
			Contract.Requires(next != null);
			Contract.Assume(next != this);
			Follower = next;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		public ThreadingFilter(IDataProcessor<T> next, TaskScheduler scheduler)
			: this(next, scheduler, TaskCreationOptions.None)
		{
			Contract.Requires(next != null);
			Contract.Requires(scheduler != null);
			Contract.Assume(next != this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		/// <param name="options">Die zu verwendenden Task-Erzeugungsoptionen</param>
		public ThreadingFilter(IDataProcessor<T> next, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(next != null);
			Contract.Requires(scheduler != null);
			Contract.Assume(next != this);

			Follower = next;
			_scheduler = scheduler;
			_options = options;
		}

		/// <summary>
		/// Führt die Weit
		/// </summary>
		/// <param name="input"></param>
		public override void Process(T input)
		{
			// Follower ermitteln
			IDataProcessor<T> follower = Follower;

			// Kopieren
			SetProcessingState(ProcessState.Filtering, input);
			T result = Filter(input);

			// Delegat erzeugen und aufrufen.
			SetProcessingState(ProcessState.Dispatching, input);

			// Neuen Task erzeugen
			Action action = delegate { follower.Process(result); };
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
	/// Helfermethoden für <see cref="ThreadingFilter{T}"/>
	/// </summary>
	public static class ThreadingFilterHelper
	{
		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ThreadingFilter<T> MakeThreaded<T>(this IFilter<T, T> processor)
		{
			Contract.Requires(processor != null);
			Contract.Requires(!(processor is ThreadingFilter<T>));
			Contract.Requires(!(processor is TeeFilter<T>));
			Contract.Ensures(Contract.Result<ThreadingFilter<T>>() != null);
			return new ThreadingFilter<T>(processor);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ThreadingFilter<T> MakeThreaded<T>(this IFilter<T, T> processor, TaskScheduler scheduler)
		{
			Contract.Requires(processor != null);
			Contract.Requires(scheduler != null);
			Contract.Requires(!(processor is ThreadingFilter<T>));
			Contract.Requires(!(processor is TeeFilter<T>));
			Contract.Ensures(Contract.Result<ThreadingFilter<T>>() != null);
			return new ThreadingFilter<T>(processor, scheduler);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <param name="options">Die Task-Erzeugungsoptionen</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ThreadingFilter<T> MakeThreaded<T>(this IFilter<T, T> processor, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(processor != null);
			Contract.Requires(scheduler != null);
			Contract.Requires(!(processor is ThreadingFilter<T>));
			Contract.Requires(!(processor is TeeFilter<T>));
			Contract.Ensures(Contract.Result<ThreadingFilter<T>>() != null);
			return new ThreadingFilter<T>(processor, scheduler, options);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ThreadingFilter<T> MakeThreaded<T>(this TeeFilter<T> processor)
		{
			Contract.Requires(processor != null);
			Contract.Ensures(Contract.Result<ThreadingFilter<T>>() != null);
			return new ThreadingFilter<T>(processor);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ThreadingFilter<T> MakeThreaded<T>(this TeeFilter<T> processor, TaskScheduler scheduler)
		{
			Contract.Requires(processor != null);
			Contract.Requires(scheduler != null);
			Contract.Ensures(Contract.Result<ThreadingFilter<T>>() != null);
			return new ThreadingFilter<T>(processor, scheduler);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="processor">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <param name="options">Die Task-Erzeugungsoptionen</param>
		/// <returns>Ein Threadingfilter</returns>
		public static ThreadingFilter<T> MakeThreaded<T>(this TeeFilter<T> processor, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(processor != null);
			Contract.Requires(scheduler != null);
			Contract.Ensures(Contract.Result<ThreadingFilter<T>>() != null);
			return new ThreadingFilter<T>(processor, scheduler, options);
		}
	}
}

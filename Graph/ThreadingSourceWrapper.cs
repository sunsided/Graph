using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Graph
{
	/// <summary>
	/// Filter, welches die Weiterverarbeitung in einem neuen Thread durchführt.
	/// </summary>
	/// <typeparam name="T">Ein- und Ausgabeparameter</typeparam>
	public sealed class ThreadingSourceWrapper<T> : StateBase, ISource<T>
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
		/// Die Datenquelle
		/// </summary>
		private readonly ISource<T> _source;

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		public ThreadingSourceWrapper(ISource<T> next)
		{
			Contract.Requires(next != null);
			Contract.Assume(next != this);
			
			_source = next;
			_source.StateChanged += (sender, args) => SetProcessingState(args.State, args.Input);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadingFilter&lt;TIn&gt;"/> class.
		/// </summary>
		/// <param name="next">Das nachfolgende Element</param>
		/// <param name="scheduler">Der zu verwendende Task Scheduler</param>
		public ThreadingSourceWrapper(ISource<T> next, TaskScheduler scheduler)
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
		public ThreadingSourceWrapper(ISource<T> next, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(next != null);
			Contract.Requires(scheduler != null);
			Contract.Assume(next != this);

			_source = next;
			_scheduler = scheduler;
			_options = options;

			_source.StateChanged += (sender, args) => SetProcessingState(args.State, args.Input);
		}

		/// <summary>
		/// Erzeugt Daten
		/// </summary>
		public void Process()
		{
			// Neuen Task erzeugen
			Action action = delegate { _source.Process(); };
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

		/// <summary>
		/// Hängt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="T">Eingabedatentyp des nächsten Elementes</typeparam>
		/// <param name="element">Der anzuhängende Filter</param>
		void IAppendable<T>.Append(IDataProcessor<T> element)
		{
			_source.Append(element);
		}

		/// <summary>
		/// Hängt einen Filter oder eine Senke an
		/// </summary>
		/// <typeparam name="TOutNext">Eingabedatentyp des nächsten Elementes</typeparam>
		/// <param name="element">Der anzuhängende Filter</param>
		IAppendable<TOutNext> IAppendable<T>.Append<TOutNext>(IAppendableDataProcessor<T, TOutNext> element)
		{
			return _source.Append(element);
		}

		/// <summary>
		/// Erzeugt die Eingabe
		/// </summary>
		/// <rereturns>Die Ausgabedaten</rereturns>
		T ISource<T>.Create()
		{
			return _source.Create();
		}
	}

	/// <summary>
	/// Helfermethoden für <see cref="ThreadingSourceWrapper{T}"/>
	/// </summary>
	public static class ThreadingSourceHelper
	{
		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="source">Der zu wrappende Processor</param>
		/// <returns>Eine Threading-Source</returns>
		public static ThreadingSourceWrapper<T> MakeThreaded<T>(this ISource<T> source)
		{
			Contract.Requires(source != null);
			Contract.Requires(!(source is ThreadingSourceWrapper<T>));
			Contract.Ensures(Contract.Result<ThreadingSourceWrapper<T>>() != null);
			return new ThreadingSourceWrapper<T>(source);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="source">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <returns>Eine Threading-Source</returns>
		public static ThreadingSourceWrapper<T> MakeThreaded<T>(this ISource<T> source, TaskScheduler scheduler)
		{
			Contract.Requires(source != null);
			Contract.Requires(scheduler != null);
			Contract.Requires(!(source is ThreadingSourceWrapper<T>));
			Contract.Ensures(Contract.Result<ThreadingSourceWrapper<T>>() != null);
			return new ThreadingSourceWrapper<T>(source, scheduler);
		}

		/// <summary>
		/// Führt die Verarbeitung des Elementes in einem <see cref="Task"/> aus.
		/// </summary>
		/// <typeparam name="T">Der zu verarbeitende Datentyp</typeparam>
		/// <param name="source">Der zu wrappende Processor</param>
		/// <param name="scheduler">Der zu verwendende Scheduler</param>
		/// <param name="options">Die Task-Erzeugungsoptionen</param>
		/// <returns>Eine Threading-Source</returns>
		public static ThreadingSourceWrapper<T> MakeThreaded<T>(this ISource<T> source, TaskScheduler scheduler, TaskCreationOptions options)
		{
			Contract.Requires(source != null);
			Contract.Requires(scheduler != null);
			Contract.Requires(!(source is ThreadingSourceWrapper<T>));
			Contract.Ensures(Contract.Result<ThreadingSourceWrapper<T>>() != null);
			return new ThreadingSourceWrapper<T>(source, scheduler, options);
		}
	}
}
